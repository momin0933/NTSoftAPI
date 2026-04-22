using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using NTSoftMerchantAPI.BusinessLayer.Service;
using NTSoftMerchantAPI.Models;

namespace NTSoftMerchantAPI.Controllers
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
        public async Task<IActionResult> Post([FromBody] RptUserAccount _userData)
        {
            if (_userData != null && _userData.UserId != null && _userData.password != null)
            {
                var user = GetUser(_userData.UserId, _userData.password);

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

        [AllowAnonymous]
        [Route("api/EcomToken")]
        [HttpPost]
        public async Task<IActionResult> EcomPost([FromBody] EcomRptUserAccount _userData)
        {
            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                //var user = await GetUser(_userData.email, _userData.password);
                var user = EcomGetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    // password blank
                    user.Password = null;

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //create claims details based on the user information
                    UserAccount UserData = new UserAccount();
                    var claims = new[] {

                        //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        //new Claim("UserLogin", user.UserLogin.ToString()),
                        //new Claim("FullName", user.FullName.ToString()),                       
                        //new Claim("Email", user.Email.ToString())
                        new Claim(ClaimTypes.NameIdentifier,user.Email),
                        //new Claim(ClaimTypes.GivenName,user.username),
                        //new Claim(ClaimTypes.Email,user.Email),
                        //new Claim(ClaimTypes.Surname,user.FullName),
                        //new Claim(ClaimTypes.Role,user.UserRole),
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

                    //return Ok(new JwtSecurityTokenHandler().WriteToken(token));
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



        [Route("api/tokenvalidate")]
        [HttpPost]
        public UserAccount tokenvalidate()
        {
            UserAccount user = new UserAccount();            
            //const string HeaderKeyName = "Authorization";
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var token = headerValue.Parameter;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;
                var email = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
                //user = _context.UserAccounts.FirstOrDefault(x => x.Email == email);               
            }          
          
            return user;
        }
        
        [Route("api/tokenvalidateByGet")]
        [HttpGet]
        public UserAccount tokenvalidateByGet()
        {
            UserAccount user = new UserAccount();
            //const string HeaderKeyName = "Authorization";
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var token = headerValue.Parameter;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;
                var email = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
               // user = _context.UserAccounts.FirstOrDefault(x => x.Email == email);
            }

            return user;
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

        private EcomUser EcomGetUser(string userid, string UserPassword)
        {


            string Query = "select * from EcomtblCustomer WHERE Email = '" + userid + "' AND Password = '" + UserPassword + "' ";
            return _dapperService.GetAllByQuery<EcomUser>(Query).FirstOrDefault();
 ;
        }
    }
}
