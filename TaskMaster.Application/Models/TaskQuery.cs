using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Models;

public sealed class TaskQuery
{
    public string? SearchTerm { get; init; }
    public TaskMaster.Domain.Entities.TaskStatus? Status { get; init; }
    public DateTime? DueBefore { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
