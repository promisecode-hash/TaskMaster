using Microsoft.AspNetCore.Mvc;
using TaskMaster.Api.Models;
using TaskMaster.Api.Services;

namespace TaskMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtTokenService tokenService, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login failed: Invalid model state for user: {Username}", request.Username);
            return BadRequest(new ApiResponse<object> 
            { 
                Success = false,
                Message = "Invalid request",
                Errors = ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())
            });
        }

        var (isValid, email, name) = ValidateUser(request.Username, request.Password);
        if (!isValid)
        {
            _logger.LogWarning("Login failed: Invalid credentials for user: {Username}", request.Username);
            return Unauthorized(new ApiResponse<object> 
            { 
                Success = false, 
                Message = "Invalid username or password" 
            });
        }

        var token = _tokenService.CreateToken(request.Username, email, name);
        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        
        return Ok(new ApiResponse<AuthResponse> 
        { 
            Data = new AuthResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            },
            Success = true,
            Message = "Login successful"
        });
    }

    private static (bool IsValid, string Email, string Name) ValidateUser(string username, string password)
    {
        if (username == "admin" && password == "Password123!")
        {
            return (true, "admin@example.com", "Admin User");
        }

        return (false, string.Empty, string.Empty);
    }
}
