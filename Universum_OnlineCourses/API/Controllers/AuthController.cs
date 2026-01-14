using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Aplication.Services;
using Universum_OnlineCourses.Aplication.DTOs;
using Universum_OnlineCourses.Domain.Entities;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");

            if (await _context.Users.AnyAsync(x => x.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                LastName = dto.LastName,              
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2


            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken = refreshTokenValue
            });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRefreshDto dto)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == dto.RefreshToken &&
                    !x.IsRevoked &&
                    x.ExpiresAt > DateTime.UtcNow);

            if (storedToken == null)
                return Unauthorized();

            storedToken.IsRevoked = true;

            var user = await _context.Users.FindAsync(storedToken.UserId);

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshTokenValue
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }

}
