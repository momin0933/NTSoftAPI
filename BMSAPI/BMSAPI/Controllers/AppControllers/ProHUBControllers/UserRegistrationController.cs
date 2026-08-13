using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistration _userRegistrationService;
        private readonly ILogger<UserRegistrationController> _logger;

        public UserRegistrationController(IUserRegistration userRegistrationService, ILogger<UserRegistrationController> logger)
        {
            _userRegistrationService = userRegistrationService ?? throw new ArgumentNullException(nameof(userRegistrationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/RegisterUser")]
        public IActionResult RegisterUser([FromBody] UserRegistration model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Registration data is required" });

                if (string.IsNullOrWhiteSpace(model.Mail) || string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest(new { success = false, message = "Name, Phone, Mail, and Password are required" });

                if (_userRegistrationService.IsEmailExists(model.Mail))
                    return Conflict(new { success = false, message = "An account with this email already exists" });

                if (_userRegistrationService.IsPhoneExists(model.Phone))
                    return Conflict(new { success = false, message = "An account with this phone number already exists" });

                var result = _userRegistrationService.RegisterUser(model);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Registration failed, please try again" });

                return Ok(new { success = true, data = result, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with Mail: {Mail}", model?.Mail);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/CheckEmailExists")]
        public IActionResult CheckEmailExists(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new { success = false, message = "Email is required" });

                var exists = _userRegistrationService.IsEmailExists(email);
                return Ok(new { success = true, data = exists, message = exists ? "Email already registered" : "Email is available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/CheckPhoneExists")]
        public IActionResult CheckPhoneExists(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return BadRequest(new { success = false, message = "Phone number is required" });

                var exists = _userRegistrationService.IsPhoneExists(phone);
                return Ok(new { success = true, data = exists, message = exists ? "Phone number already registered" : "Phone number is available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking phone existence: {Phone}", phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}