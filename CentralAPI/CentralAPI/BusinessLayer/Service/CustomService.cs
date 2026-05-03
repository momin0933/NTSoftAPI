using Microsoft.IdentityModel.Tokens;
using CentralAPI.BusinessLayer.Interface;
using CentralAPI.BusinessLayer.TenantService;
using CentralAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CentralAPI.BusinessLayer.Service
{
    public class CustomService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;

        public CustomService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = config;
        }
       
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                                   .Replace("+", "-")
                                   .Replace("/", "_")
                                   .Replace("=", "");
           
        }
        public JwtSecurityToken GenerateToken(UserAccount user)
        {
            user.Password = null;
            //var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
            string tenantId = null;
            // 1. LOGIN TIME (NO TOKEN)
            tenantId = _httpContextAccessor.HttpContext?.Request?.Headers["TenantId"].FirstOrDefault();

            // 2. AFTER LOGIN (TOKEN)
            if (string.IsNullOrEmpty(tenantId))
            {
                tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //create claims details based on the user information
            UserAccount UserData = new UserAccount();
            var claims = new[]
                    {
                        new Claim("TenantId", tenantId),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
                        new Claim(ClaimTypes.Role, user.UserRole),
                    };


            var accessToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn);
            return accessToken;
        }

       
    }
}
