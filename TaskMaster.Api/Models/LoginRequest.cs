using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Api.Models;

public sealed class LoginRequest
{
    [Required]
    public string Username { get; init; } = null!;

    [Required]
    public string Password { get; init; } = null!;
}
