using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAppELK.Data;
using TodoAppELK.Models.Domain;
using TodoAppELK.Models.DTOs;

namespace TodoAppELK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TodoDbContext _dbContext;
        //private readonly ILogger<AuthController> _logger;

        public AuthController(TodoDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == dto.Username))
            {
                Log.Error("Username {Username} already exists", dto.Username);
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (existingUser == null)
            {
                Log.Error("Login failed for username {Username}: User not found", dto.Username);
                return Unauthorized("Invalid username or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash))
            {
                Log.Error("Login failed for username {Username}: Invalid password", dto.Username);
                return Unauthorized("Invalid username or password");
            }

            Log.Information("User {Username} logged in successfully", dto.Username);
            var token = GenerateJwtToken(existingUser);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
