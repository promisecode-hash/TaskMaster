using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Api.Models;
using TaskMaster.Api.Security;
using TaskMaster.Api.Services;

namespace TaskMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly AuthSettings _authSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtTokenService tokenService,
        IOptions<AuthSettings> authOptions,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _authSettings = authOptions.Value;
        _jwtSettings = jwtOptions.Value;
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

        var isValid = ValidateUser(request.Username, request.Password);
        if (!isValid)
        {
            _logger.LogWarning("Login failed: Invalid credentials for user: {Username}", request.Username);
            return Unauthorized(new ApiResponse<object> 
            { 
                Success = false, 
                Message = "Invalid username or password" 
            });
        }

        var token = _tokenService.CreateToken(_authSettings.Username, _authSettings.Email, _authSettings.Name);
        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        
        return Ok(new ApiResponse<AuthResponse> 
        { 
            Data = new AuthResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            },
            Success = true,
            Message = "Login successful"
        });
    }

    private bool ValidateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(_authSettings.Username) ||
            string.IsNullOrWhiteSpace(_authSettings.PasswordHash))
        {
            _logger.LogError("Login failed: Auth settings are not configured");
            return false;
        }

        return string.Equals(username, _authSettings.Username, StringComparison.Ordinal) &&
            BCrypt.Net.BCrypt.Verify(password, _authSettings.PasswordHash);
    }
}
