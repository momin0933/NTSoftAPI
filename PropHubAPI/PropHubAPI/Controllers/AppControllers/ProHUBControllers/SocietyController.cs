using Microsoft.AspNetCore.Mvc;
using PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB;

namespace PropHubAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class SocietyController : ControllerBase
    {
        private readonly ISociety _service;
        private readonly ILogger<SocietyController> _logger;

        public SocietyController(ISociety service, ILogger<SocietyController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Public (used on the registration screen, before login).
        [HttpGet("api/GetSocietyList")]
        public IActionResult GetSocietyList()
        {
            try
            {
                var list = _service.GetSocietyList();
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving society list");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
