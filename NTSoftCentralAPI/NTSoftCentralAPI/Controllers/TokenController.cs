using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using NTSoftCentralAPI.BusinessLayer.Service;
using NTSoftCentralAPI.BusinessLayer.TenantService;
using NTSoftCentralAPI.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace NTSoftCentralAPI.Controllers
{
   
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperService _dapperService;
        private readonly NTSoftDbContextFactory _factory;
        private readonly CustomService _customService;
        private readonly ICommonService _commonService;

        public TokenController(IConfiguration config,NTSoftDbContextFactory factory,IDapperService dapperService,CustomService customService,ICommonService commonService)
        {
            _configuration = config;
            _factory = factory;
            _dapperService = dapperService;
            _customService = customService;
            _commonService = commonService;
        }
        [AllowAnonymous]
        [Route("api/token")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RptUserAccount _userData)
        {
            if (_userData != null && _userData.UserId != null && _userData.password != null)
            {
                // 1. Validate user
                var user = GetUser(_userData.UserId, _userData.password);
                
                // 2. Generate tokens
                var accessToken = _customService.GenerateToken(user);
                var refreshToken = _customService.GenerateRefreshToken();
                
                // 3. Store refresh token in DB
                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false
                };
                _commonService.Add(refreshTokenEntity);

                return Ok(new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                    expiration = accessToken.ValidTo.ToLocalTime(),
                    refreshToken,
                    userData = new
                    {
                        userId = user.UserId,
                        role = user.UserRole,
                        Name = user.Name
                    }
                });
            }
            else
            {
                return BadRequest();
            }
        }

                //if (user != null)
                //{
                //    // password blank
                //    user.Password = null;
                //    var tenant = HttpContext.Items["Tenant"] as Tenant;
                //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                //    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //    //create claims details based on the user information
                //    UserAccount UserData = new UserAccount();
                //    var claims = new[] {








                //    //return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                //}
                //else
                //{
                //    return BadRequest("Invalid credentials");
                //}

        [AllowAnonymous]
        [Route("api/RefreshToken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var storedToken = _commonService.GetAll<RefreshToken>().FirstOrDefault(x => x.Token == refreshToken);

            if (storedToken == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            if (storedToken.IsRevoked)
            {
                return Unauthorized(new { message = "Token revoked" });
            }

            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Token expired" });
            }

            // 🔥 Revoke old token
            storedToken.IsRevoked = true;
            storedToken.IsActive = false;
            _commonService.Update<RefreshToken>(storedToken);

            // 🔥 Generate new refresh token
            var newrefreshToken = _customService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newrefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

           // _commonService.Add(refreshTokenEntity);
            RefreshTokenAdd(refreshTokenEntity);

            // 🔥 Generate new access token
            var user = GetUserByUserId(storedToken.UserId);
            var newAccessToken = _customService.GenerateToken(user);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                expiration = newAccessToken.ValidTo.ToLocalTime(),
                refreshToken = newrefreshToken,   // ✅ FIXED
                userData = user
            });
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
                        expires: DateTime.UtcNow.AddMinutes(1),
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
            //string Query = "select * from tbluserAccount WHERE UserId = '"+ userid + "' AND password = '"+UserPassword+"' ";
            string query = "SELECT * FROM tblUserAccount WHERE UserId = @UserId AND Password=@Password";

            var user = _dapperService.GetSingle<UserAccount>(query, new
            {
                UserId = userid,
                Password = UserPassword
            });
            if (user == null)
            {
                return null;
            }
            return user;
            // return _dapperService.GetAllByQuery<UserAccount>(Query).FirstOrDefault();            
        }
        private int RefreshTokenAdd(RefreshToken entity)
        {
            var refreshTokenEntity = new RefreshToken
            {
                UserId = entity.UserId,
                Token = entity.Token,
                ExpiryDate = entity.ExpiryDate,
                IsRevoked = entity.IsRevoked
            };
            
            return _commonService.Add(refreshTokenEntity, true);
        }
        private UserAccount GetUserByUserId(string userid)
        {
            string query = "SELECT * FROM tblUserAccount WHERE UserId = @UserId";
            return _dapperService.GetSingle<UserAccount>(query, new { UserId = userid });
        }
        private EcomUser EcomGetUser(string email, string password)
        {
            string query = "SELECT * FROM EcomtblCustomer WHERE Email = @Email AND Password = @Password";
            return _dapperService.GetSingle<EcomUser>(query, new { Email = email, Password = password });
        }
    }
}
