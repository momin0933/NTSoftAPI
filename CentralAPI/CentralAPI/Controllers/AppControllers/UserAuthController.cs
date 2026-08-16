using CentralAPI.BusinessLayer.Interface.IApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAPI.Controllers.AppControllers
{
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuth _userAuthService;
        private readonly ILogger<UserAuthController> _logger;
        public UserAuthController(IUserAuth userAuthService, ILogger<UserAuthController> logger)
        {
            _userAuthService = userAuthService ?? throw new ArgumentNullException(nameof(userAuthService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public class LoginRequest
        {
            public string Phone { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        [AllowAnonymous]
        [Route("api/LoginUser")]
        [HttpPost]
        public IActionResult LoginUser([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { success = false, message = "Phone and Password are required" });

            try
            {
                var result = _userAuthService.Login(request.Phone, request.Password);
                if (result == null)
                    return Unauthorized(new { success = false, message = "Invalid phone number or password" });

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    expiration = result.Expiration,
                    refreshToken = result.RefreshToken,
                    userData = result.User,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user with Phone: {Phone}", request?.Phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [Authorize]
        [Route("api/RefreshUserToken")]
        [HttpPost]
        public IActionResult RefreshUserToken(string rToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rToken))
                    return BadRequest(new { success = false, message = "rToken is required" });

                var result = _userAuthService.RefreshToken(rToken);
                if (result == null)
                    return Unauthorized(new { message = "Invalid or expired refresh token" });

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    expiration = result.Expiration,
                    refreshToken = result.RefreshToken,
                    userData = result.User,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [Authorize]
        [Route("api/LogoutUser")]
        [HttpPost]
        public IActionResult LogoutUser(string rToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rToken))
                    return BadRequest(new { success = false, message = "rToken is required" });

                var result = _userAuthService.Logout(rToken);
                if (!result)
                    return Unauthorized(new { message = "Invalid or already-revoked refresh token" });

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
