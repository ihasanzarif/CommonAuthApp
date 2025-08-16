using CommonAuthApp.API.Data;
using CommonAuthApp.API.Handlers;
using CommonAuthApp.API.Models;
using CommonAuthApp.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommonAuthApp.API.Controllers
{
    [Route("admin/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuation;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuation, IAuthService authService)
        {
            _configuation = configuation;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid login request");
            }

            var user = await _authService.GetUserByLogin(model.Username, model.Password);
            if (user is null || !PasswordHashHandlers.CheckPassword(model.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(GenerateJwtToken(user.Username));
        }

        private LoginResponseModel GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

            string secret = _configuation.GetValue<string>("Jwt:Secret");
            string issuer = _configuation.GetValue<string>("Jwt:Issuer");
            string audience = _configuation.GetValue<string>("Jwt:Audience");
            var tokenExpiryInMinutes = _configuation.GetValue<int>("Jwt:TokenExpiryInMinutes");
            var setTokenExpiration = DateTime.UtcNow.AddHours(tokenExpiryInMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: setTokenExpiration,
                signingCredentials: creds
                );
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponseModel
            {
                Username = username,
                Token = tokenHandler,
                TokenExpired = setTokenExpiration.Ticks,
                RefreshToken = Guid.NewGuid().ToString() // Generate a new refresh token
            };
        }
    }
}
