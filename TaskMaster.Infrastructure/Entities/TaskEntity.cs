using TaskMaster.Domain.Entities;

namespace TaskMaster.Infrastructure.Entities;

public sealed class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; }

    public TaskItem ToDomain()
    {
        var task = new TaskItem(Title, Description, DueDate)
        {
            Id = Id,
            CreatedAt = CreatedAt
        };

        task.SetStatus(Status);
        return task;
    }

    public static TaskEntity FromDomain(TaskItem task)
    {
        return new TaskEntity
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status
        };
    }
}
