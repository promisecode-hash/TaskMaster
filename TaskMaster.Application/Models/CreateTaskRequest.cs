using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Application.Models;

public sealed class CreateTaskRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = null!;

    [StringLength(1000)]
    public string? Description { get; init; }

    public DateTime? DueDate { get; init; }
}

