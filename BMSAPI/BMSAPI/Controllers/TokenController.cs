using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BMSAPI.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperService _dapper;

        public TokenController(IConfiguration configuration, IDapperService dapper)
        {
            _configuration = configuration;
            _dapper = dapper;
        }

        [AllowAnonymous]
        [Route("api/token")]
        [HttpPost]
        public IActionResult Post([FromBody] RptUserAccount userData)
        {
            if (userData == null ||
                string.IsNullOrWhiteSpace(userData.UserId) ||
                string.IsNullOrWhiteSpace(userData.password))
            {
                return BadRequest();
            }

            var user = GetUser(userData.UserId, userData.password);
            if (user == null)
                return BadRequest("Invalid credentials");

            user.Password = null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId ?? ""),
                new Claim(ClaimTypes.Role, user.UserRole ?? "")
            };

            var accessToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(25),
                signingCredentials: signIn);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                expiration = accessToken.ValidTo,
                userData = user
            });
        }

        [Authorize]
        [Route("api/tokenvalidate")]
        [HttpPost]
        public IActionResult TokenValidate()
        {
            return GetCurrentUser();
        }

        [Authorize]
        [Route("api/tokenvalidateByGet")]
        [HttpGet]
        public IActionResult TokenValidateByGet()
        {
            return GetCurrentUser();
        }

        private IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = _dapper.GetSingle<UserAccount>(
                "SELECT * FROM tblUserAccount WHERE UserId = @UserId",
                new { UserId = userId });

            if (user == null)
                return NotFound();

            user.Password = null;
            return Ok(user);
        }

        private UserAccount? GetUser(string userId, string password)
        {
            // Parameterized query: user input কখনো SQL হিসেবে চলবে না
            return _dapper.GetSingle<UserAccount>(
                "SELECT * FROM tblUserAccount WHERE UserId = @UserId AND Password = @Password",
                new { UserId = userId, Password = password });
        }
    }
}