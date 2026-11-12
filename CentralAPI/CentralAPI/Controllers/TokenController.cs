using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using CentralAPI.BusinessLayer.Service;
using CentralAPI.BusinessLayer.TenantService;
using CentralAPI.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace CentralAPI.Controllers
{

    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperService _dapperService;
        private readonly NTSoftDbContextFactory _factory;
        private readonly CustomService _customService;
        private readonly ICommonService _commonService;

        public TokenController(IConfiguration config, NTSoftDbContextFactory factory, IDapperService dapperService, CustomService customService, ICommonService commonService)
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

                // wrong user id / password: answer 401 (before, GenerateToken(null) threw and the caller got a 500)
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid user id or password" });
                }

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
                    userData = BuildUserData(user)
                });
            }
            else
            {
                return BadRequest();
            }
        }


        [Authorize]
        [Route("api/RefreshToken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string rToken)
        {
            var storedToken = _commonService.GetAll<RefreshToken>().FirstOrDefault(x => x.Token == rToken);

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

            // the account was deleted after the token was issued
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var newAccessToken = _customService.GenerateToken(user);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                expiration = newAccessToken.ValidTo.ToLocalTime(),
                refreshToken = newrefreshToken,   // ✅ FIXED
                userData = BuildUserData(user)    // same fields as the login, so the profile picture survives a refresh
            });
        }

        [Authorize]
        [Route("api/logout")]
        [HttpPost]
        public IActionResult Logout(string rToken)
        {
            var storedToken = _commonService.GetAll<RefreshToken>().FirstOrDefault(x => x.Token == rToken);

            if (storedToken == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            if (storedToken.IsRevoked)
            {
                return Unauthorized(new { message = "Token already revoked" });
            }

            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Token expired" });
            }

            // 🔥 Revoke token
            storedToken.IsRevoked = true;
            storedToken.IsActive = false;

            _commonService.Update(storedToken);

            return Ok(new { message = "Logout successful" });
        }

        /// <summary>
        /// The "userData" block of the login and the refresh response — one place, so both always match.
        /// Only fields that are safe to show the user are listed. To send one more column, add it here
        /// (never Password / ReTypePassword).
        /// </summary>
        private static object BuildUserData(UserAccount user)
        {
            return new
            {
                userId = user.UserId,
                role = user.UserRole,
                UserRole = user.UserRole,
                Name = user.Name,
                CompId = user.CompId,
                SMSAppName = user.SMSAppName,

                // profile
                ProfilePicture = user.ProfilePicture,
                Designation = user.Designation,
                Department = user.Department,
                Email = user.Email,
                MobileNo = user.MobileNo,
                NickName = user.NickName,
                FullName = user.FullName,
                EmployeeId = user.EmployeeId
            };
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

    }
}