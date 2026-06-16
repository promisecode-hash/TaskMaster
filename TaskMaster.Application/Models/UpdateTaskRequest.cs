using System.ComponentModel.DataAnnotations;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Models;

public sealed class UpdateTaskRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = null!;

    [StringLength(1000)]
    public string? Description { get; init; }

    public DateTime? DueDate { get; init; }
    public TaskMaster.Domain.Entities.TaskStatus Status { get; init; } = TaskMaster.Domain.Entities.TaskStatus.Backlog;
}

