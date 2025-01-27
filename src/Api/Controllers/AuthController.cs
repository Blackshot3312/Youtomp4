using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Mvc;
using Services;
using BCrypt.Net;
using Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (_context.Users.Any(u => u.Username == request.Username))
            return BadRequest("O usuário já existe");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return BadRequest("Usuário não encontrado");
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return BadRequest("Senha inválida");

        var token = _authService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

