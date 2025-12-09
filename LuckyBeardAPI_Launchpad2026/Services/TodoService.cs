using LuckyBeardAPI_Launchpad2026.Data;
using LuckyBeardAPI_Launchpad2026.DTOs;
using LuckyBeardAPI_Launchpad2026.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LuckyBeardAPI_Launchpad2026.Services
{
    public class TodoService
    {
        private readonly ApplicationDbContext _db;
        public TodoService(ApplicationDbContext db) { _db = db; }

        //list all f the users todos with a status filter
        public async Task<PagedResult<TodoDto>> GetAsync(string userId, TodosDto q, CancellationToken ct)
        {
            var query = _db.ToDos.AsNoTracking().Where(t => t.UserToDoId == userId);

            //status filter
            if (q.Status.HasValue) {
                query = query.Where(t => t.Status == q.Status.Value);
            }

            //total number of todos
            var total = await query.CountAsync(ct);

            //create list using TodoDto
            var items = await query             
            .OrderByDescending(t => t.Updated_At) //listed by most recently updated
            .Select(t => new TodoDto(t.ToDoId, 
            t.Title, 
            t.ToDoDescription, 
            t.Status, 
            t.Created_At, 
            t.Updated_At))
            .ToListAsync(ct);


            return new PagedResult<TodoDto>(items, total);
        }

        //get todo by its ID
        public async Task<TodoDto?> GetByIdAsync(string userId, string id, CancellationToken ct)
        {
            return await _db.ToDos.AsNoTracking()
            .Where(t => t.UserToDoId == userId && t.ToDoId == id)
            .Select(t => new TodoDto(
            t.ToDoId, 
            t.Title, 
            t.ToDoDescription, 
            t.Status, 
            t.Created_At, 
            t.Updated_At))
            .SingleOrDefaultAsync(ct);
        }

        //create a new todo for user
        public async Task<TodoDto> CreateAsync(string id, string userId, CreateTodoDto dto, CancellationToken ct)
        {
            //entity fusing dto as a base
            var todo = new ToDoModel
            {
                ToDoId = id,
                UserToDoId = userId,
                Title = dto.Title.Trim(),
                ToDoDescription = dto.Description?.Trim(),
                Status = dto.Status
            };
            _db.ToDos.Add(todo);
            await _db.SaveChangesAsync(ct);
            return new TodoDto(
                todo.ToDoId, 
                todo.Title, 
                todo.ToDoDescription, 
                todo.Status, 
                todo.Created_At, 
                todo.Updated_At);
        }

        //update existing todo
        public async Task<TodoDto?> UpdateAsync(string userId, string id, UpdateTodoDto dto, CancellationToken ct)
        {
            // fetch only one todo that exists and belongs to user
            var todo = await _db.ToDos.SingleOrDefaultAsync(t => t.ToDoId == id && t.UserToDoId == userId, ct);
            if (todo == null){
                return null; 
            }

            //state validation
            if (string.IsNullOrWhiteSpace(dto.Title)) { 
                throw new InvalidOperationException("Title is required"); 
            }

            todo.Title = dto.Title.Trim();
            todo.ToDoDescription = dto.Description?.Trim();
            todo.Status = dto.Status;
            todo.Updated_At = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return new TodoDto(todo.ToDoId, todo.Title, todo.ToDoDescription, todo.Status, todo.Created_At, todo.Updated_At);
        }

        //upload a file that contains multiple todos
        public async Task<int> BulkUploadAsync(string userId, IFormFile file, CancellationToken ct)
        {
            //ensure file is not empty
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File is required.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var todosToAdd = new List<ToDoModel>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            if (extension == ".json")
            {
                //file structure must be an array of CreateTodoDto
                var text = await reader.ReadToEndAsync();
                var items = JsonSerializer.Deserialize<List<CreateTodoDto>>(text);

                if (items == null)
                    throw new InvalidOperationException("Invalid JSON format.");

                foreach (var dto in items)
                {
                    var todo = new ToDoModel
                    {
                        ToDoId = Guid.NewGuid().ToString(),
                        UserToDoId = userId,
                        Title = dto.Title.Trim(),
                        ToDoDescription = dto.Description?.Trim(),
                        Status = dto.Status,
                        Created_At = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    todosToAdd.Add(todo);
                }
            }
            else if (extension == ".csv")
            {
                // first line is a header row then: Title,Description,Status
                bool firstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;

                    // skip header
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');

                    if (parts.Length < 3)
                        continue;

                    var title = parts[0].Trim();
                    var desc = parts[1].Trim();
                    var statusText = parts[2].Trim();

                    if (!Enum.TryParse<TodoStatus>(statusText, ignoreCase: true, out var status))
                    {
                        // default to Pending when invalid
                        status = TodoStatus.Pending;
                    }

                    var todo = new ToDoModel
                    {
                        ToDoId = Guid.NewGuid().ToString(),
                        UserToDoId = userId,
                        Title = title,
                        ToDoDescription = string.IsNullOrWhiteSpace(desc) ? null : desc,
                        Status = status,
                        Created_At = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    todosToAdd.Add(todo);
                }
            }
            else
            {
                throw new InvalidOperationException("Only .json and .csv files are supported.");
            }

            if (todosToAdd.Count == 0)
                return 0;

            await _db.ToDos.AddRangeAsync(todosToAdd, ct);
            await _db.SaveChangesAsync(ct);

            return todosToAdd.Count;
        }

        //soft delete todo
        public async Task<bool> DeleteAsync(string userId, string id, CancellationToken ct)
        {
            // soft delete (set IsDeleted) by the authroised userId
            var affected = await _db.ToDos
        .Where(t => t.ToDoId == id && t.UserToDoId == userId)
        .ExecuteUpdateAsync(
            s => s
                .SetProperty(t => t.IsDeleted, true)
                .SetProperty(t => t.Updated_At, DateTime.UtcNow), ct);

            return affected > 0;
        }

    }
}
