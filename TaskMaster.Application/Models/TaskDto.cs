using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Models;

public sealed class TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public TaskMaster.Domain.Entities.TaskStatus Status { get; init; }
}
