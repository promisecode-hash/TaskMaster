using Microsoft.AspNetCore.Mvc;
using TaskMaster.Api.Models;
using TaskMaster.Api.Services;

namespace TaskMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;

    public AuthController(IJwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsValidUser(request.Username, request.Password))
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(request.Username);
        return Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(120)
        });
    }

    private static bool IsValidUser(string username, string password)
    {
        return username == "admin" && password == "Password123!";
    }
}
