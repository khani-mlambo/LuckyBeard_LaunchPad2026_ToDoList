using LuckyBeardAPI_Launchpad2026.DTOs;
using LuckyBeardAPI_Launchpad2026.Models;
using LuckyBeardAPI_Launchpad2026.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LuckyBeardAPI_Launchpad2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly TodoService _svc;
        public TodosController(TodoService s)
        {
            _svc = s;
        }

        //view all todos made by user
        // GET: api/<TodosController>
        [HttpGet("ViewAll")]
        public async Task<PagedResult<TodoDto>> List([FromQuery] TodosDto q, CancellationToken ct)
        {
            var userId = GetUserId();
            return await _svc.GetAsync(userId, q, ct);
        }

        //return a single todo using its ID
        // GET /api/todos/{id}
        [HttpGet("View:{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var userId = GetUserId();

            var todo = await _svc.GetByIdAsync(userId, id, ct);

            return todo is null ? NotFound() : Ok(todo);
        }

        //allow user to upload a single todo
        // POST api/<TodosController>
        [HttpPost("UploadToDo")]
        public async Task<IActionResult> Post([FromBody] CreateTodoDto dto, CancellationToken ct)
        {
            var userId = GetUserId();

            // generate new id for todo
            var newId = Guid.NewGuid().ToString();

            //the rest of the creation comes from the service
            var created = await _svc.CreateAsync(newId, userId, dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        //update existing todo tht belongs to user
        // PUT api/<TodosController>/5
        [HttpPut("Update:{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTodoDto dto, CancellationToken ct)
        {
            var userId = GetUserId();

            var updated = await _svc.UpdateAsync(userId, id, dto, ct);

            return updated is null ? NotFound() : Ok(updated);
        }

        //allow user to upload a file with todos
        // POST: /api/todos/upload
        [HttpPost("UploadFile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var userId = GetUserId();

            try
            {
                var count = await _svc.BulkUploadAsync(userId, file, ct);
                return Ok(new { inserted = count });
            }
            catch (InvalidOperationException ex)
            {
                //send error message for unsupported or bad file format
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<TodosController>/5
        [HttpDelete("DeleteToDo:{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var userId = GetUserId();

            var ok = await _svc.DeleteAsync(userId, id, ct);

            return ok ? NoContent() : NotFound();
        }


        // extarct userID from jwt token
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Invalid user identity.");
        }
    }
}
