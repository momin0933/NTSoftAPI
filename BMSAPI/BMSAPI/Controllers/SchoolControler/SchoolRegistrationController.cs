
using BMSAPI.BusinessLayer.Interface.SchoolInterface;
using BMSAPI.Models.School;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class SchoolRegistrationController : ControllerBase
    {
        private readonly ISchoolRegistration _schoolRegistrationService;
        private readonly ILogger<SchoolRegistrationController> _logger;

        public SchoolRegistrationController(ISchoolRegistration schoolRegistrationService, ILogger<SchoolRegistrationController> logger)
        {
            _schoolRegistrationService = schoolRegistrationService ?? throw new ArgumentNullException(nameof(schoolRegistrationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/RegisterStudent")]
        public IActionResult RegisterStudent([FromBody] SchoolUserRegistration model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Registration data is required" });

                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Category))
                    return BadRequest(new { success = false, message = "Name, Phone, and Category are required" });

               
                var result = _schoolRegistrationService.RegisterStudent(model);

                if (!result.Success)
                    return StatusCode(500, new { success = false, message = "Registration failed, please try again" });

                var registrationId = model.RegNo;

                return Ok(new
                {
                    success = true,
                    registrationId = model.RegNo,
                    id = result.Id,
                    message = "Registration সফল হয়েছে"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering student with Phone: {Phone}", model?.Phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       

  
    }
}