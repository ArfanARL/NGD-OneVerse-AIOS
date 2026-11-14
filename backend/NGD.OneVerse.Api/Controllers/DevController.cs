using Microsoft.AspNetCore.Mvc;
using NGD.OneVerse.Api.Services;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/dev")]
public sealed class DevController : ControllerBase
{
    private readonly PasswordService _passwordService;

    public DevController(PasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    [HttpPost("hash-password")]
    public IActionResult HashPassword([FromBody] HashPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Password is required." });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "ngd.admin",
            Email = "ngd.admin@localhost"
        };

        var hash = _passwordService.HashPassword(user, request.Password);

        return Ok(new
        {
            passwordHash = hash
        });
    }

    public sealed class HashPasswordRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}
