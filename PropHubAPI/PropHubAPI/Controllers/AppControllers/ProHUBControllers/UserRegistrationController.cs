using PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using PropHubAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace PropHubAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        // Owner / Tenant (and any other non-society role) get this TenantId.
        private const string DefaultTenantValue = "PropHub";

        private readonly IUserRegistration _userRegistrationService;
        private readonly IUserRole _userRoleService;
        private readonly ILogger<UserRegistrationController> _logger;

        public UserRegistrationController(
            IUserRegistration userRegistrationService,
            IUserRole userRoleService,
            ILogger<UserRegistrationController> logger)
        {
            _userRegistrationService = userRegistrationService ?? throw new ArgumentNullException(nameof(userRegistrationService));
            _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static bool IsSocietyRole(string? role) =>
            string.Equals(role?.Trim(), "Society Member", StringComparison.OrdinalIgnoreCase);

        [HttpPost("api/RegisterUser")]
        public IActionResult RegisterUser([FromBody] UserRegistration model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Registration data is required" });

                if (string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest(new { success = false, message = "Phone and Password are required" });

                if (model.UserRoles == null || model.UserRoles.Count == 0)
                    return BadRequest(new { success = false, message = "At least one user role must be selected" });

                if (model.UserRoles.Any(IsSocietyRole) && string.IsNullOrWhiteSpace(model.SocietyTenantValue))
                    return BadRequest(new { success = false, message = "Please select your society" });

                // Email is optional — only check for a duplicate if one was provided.
                if (!string.IsNullOrWhiteSpace(model.Mail) && _userRegistrationService.IsEmailExists(model.Mail))
                    return Conflict(new { success = false, message = "An account with this email already exists" });

                if (_userRegistrationService.IsPhoneExists(model.Phone))
                    return Conflict(new { success = false, message = "An account with this phone number already exists" });

                var newUId = _userRegistrationService.RegisterUser(model);
                if (newUId <= 0)
                    return StatusCode(500, new { success = false, message = "Registration failed, please try again" });

                foreach (var role in model.UserRoles)
                {
                    if (string.IsNullOrWhiteSpace(role)) continue;

                    // Society Member -> the selected society's TenantValue; Owner / Tenant -> 'PropHub'
                    var tenantId = IsSocietyRole(role) ? model.SocietyTenantValue!.Trim() : DefaultTenantValue;

                    var roleAdded = _userRoleService.AddUserRole(newUId, role, tenantId, model.EntryBy ?? model.Phone);
                    if (!roleAdded)
                        _logger.LogWarning("Failed to add role {Role} for new UId: {UId}", role, newUId);
                }

                return Ok(new { success = true, data = true, message = "Registration successful" });
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