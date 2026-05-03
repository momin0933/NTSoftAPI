using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using HRAPI.BusinessLayer.Service;
using HRAPI.Models;

namespace HRAPI.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperService _dapperService;  // ✅ Interface use করুন
        private readonly NTSoftDbContext _context;

        public TokenController(IConfiguration config, NTSoftDbContext context, IDapperService dapperService)  // ✅ Interface
        {
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dapperService = dapperService ?? throw new ArgumentNullException(nameof(dapperService));
        }

        [AllowAnonymous]
        [Route("api/token")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserAccount _userData)
        {
            if (_userData != null && _userData.UserId != null && _userData.Password != null)
            {
                var user = GetUser(_userData.UserId, _userData.Password);

                if (user != null)
                {
                    // password blank
                    user.Password = null;

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
                        new Claim(ClaimTypes.Role, user.UserRole),
                    };

                    var accessToken = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: signIn);
                        
                    return Ok(new
                    {
                        accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                        expiration = accessToken.ValidTo,
                        userData = user
                    });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

       
        private UserAccount GetUser(string userid, string UserPassword)
        {

            // UserPassword = EncryptDecrypt.Encrypt(UserPassword);

            // SqlDataClass sqlData = new SqlDataClass();
            //var UserProjectList = _IDapperService.GetAllByQuery<VwUserDetails>(sqlData.GetUserInfo(UserLogin, UserPassword));
            string Query = "select * from tbluserAccount WHERE UserId = '"+ userid + "' AND password = '"+UserPassword+"' ";
            return _dapperService.GetAllByQuery<UserAccount>(Query).FirstOrDefault();
            //return await _context.UserAccounts.Where(x => x.email == useremail && x.password == UserPassword).AsQueryable();
            
            //UserAccount user = new UserAccount();
            //user.UserId = "momin";
            //user.Password = "1234";
            //user.UserName = "momin";
            //user.UserRole = "sysadmin";
            //return user;
        }

        
    }
}
