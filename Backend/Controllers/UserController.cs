using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TwitterClone.Models;

namespace TwitterClone.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);

            if (existingUser != null)
            {
                return BadRequest("Username is already taken bro.");
            }

            User newUser = new()
            {
                UserName = registerDto.Username,
                PasswordHash = registerDto.Password
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User registered successfully." });
        }
        [HttpPut("{username}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string username, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrEmpty(updateDto.Username) && updateDto.Username != username)
            {
                var existingUser = await _userManager.FindByNameAsync(updateDto.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username is already taken.");
                }
                user.UserName = updateDto.Username;
            }

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
                if (!passwordResult.Succeeded)
                {
                    return BadRequest(passwordResult.Errors);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok(new { Message = "User updated successfully." });
        }

        [HttpDelete("{username}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User deleted successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            User user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                claims: [],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            String tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { Token = tokenString });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            List<User> users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok(new { Message = "You're authenticated!" });
        }
    }
}
