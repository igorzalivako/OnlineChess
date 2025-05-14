using ChessLibrary.Models.DTO;
using ChessServer.Data;
using ChessServer.Models;
using ChessServer.Models.DTO;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public UsersController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        AuthResponse authResponse;

        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
        {
            authResponse = new AuthResponse() { Token = "", Success = false, Error = "Пользователь уже существует" };
            return BadRequest(authResponse);
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rating = 1000
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        var token = GenerateJwtToken(user);
        authResponse = new AuthResponse() { Token = token, Success = true, Error = "" };
        return Ok(authResponse);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        LoginResponseDto authResponse; 
        
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            authResponse = new LoginResponseDto() { Token = "", Success = false, Error = "Неверный пароль" };
            return Unauthorized(new { authResponse });
        }

        var token = GenerateJwtToken(user);
        authResponse = new LoginResponseDto() { Token = token, Success = true, Error = "", Username = user.Username, Rating = user.Rating };
        return Ok(authResponse);
    }
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username), // Используем ID вместо Username
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Username в отдельном claim
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}