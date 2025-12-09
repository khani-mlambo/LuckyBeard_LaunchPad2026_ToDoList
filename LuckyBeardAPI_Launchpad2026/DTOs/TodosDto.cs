using LuckyBeardAPI_Launchpad2026.Models;

namespace LuckyBeardAPI_Launchpad2026.DTOs
{
    public class TodosDto
    {
       public TodoStatus? Status { get; init; }
    }

    public sealed record CreateTodoDto
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public TodoStatus Status { get; init; } = TodoStatus.Pending;
    }

    public sealed record UpdateTodoDto
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public TodoStatus Status { get; init; }
    }

    public sealed record TodoDto(string Id, string Title, string? Description, TodoStatus Status, DateTime CreatedAt, DateTime UpdatedAt);

    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total);
}
