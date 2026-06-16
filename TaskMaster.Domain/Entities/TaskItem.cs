namespace TaskMaster.Domain.Entities;

public enum TaskStatus
{
    Backlog,
    InProgress,
    Completed,
    Cancelled
}

public sealed class TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DueDate { get; private set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Backlog;

    private TaskItem() { }

    public TaskItem(string title, string? description = null, DateTime? dueDate = null)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Task title must not be empty.", nameof(title))
            : title.Trim();
        Description = description?.Trim();
        DueDate = dueDate;
    }

    public void Update(string title, string? description, DateTime? dueDate)
    {
        EnsureMutableState();

        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Task title must not be empty.", nameof(title))
            : title.Trim();
        Description = description?.Trim();
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(TaskStatus status)
    {
        EnsureMutableState();

        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot change the status of a completed or cancelled task.");
        }

        Status = status;
    }

    private void EnsureMutableState()
    {
        if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
        {
            throw new InvalidOperationException("A completed or cancelled task cannot be modified.");
        }
    }
}
