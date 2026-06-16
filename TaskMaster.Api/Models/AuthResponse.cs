namespace TaskMaster.Api.Models;

public sealed class AuthResponse
{
    public string Token { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
}
