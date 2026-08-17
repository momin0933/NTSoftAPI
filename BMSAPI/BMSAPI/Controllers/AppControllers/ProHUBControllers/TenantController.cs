using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.TenantService;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenant _tenantService;
        private readonly ILogger<TenantController> _logger;

        public TenantController(ITenant tenantService, ILogger<TenantController> logger)
        {
            _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/AddTenant")]
        public IActionResult AddTenant([FromBody] TenantData model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Tenant data is required" });

                if (model.PropertyId == null || model.PropertyId <= 0)
                    return BadRequest(new { success = false, message = "A valid PropertyId is required" });

                if (model.PropDetailsId == null || model.PropDetailsId <= 0)
                    return BadRequest(new { success = false, message = "A valid PropDetailsId is required" });

                if (string.IsNullOrWhiteSpace(model.TenantName) || string.IsNullOrWhiteSpace(model.TenantPhone))
                    return BadRequest(new { success = false, message = "Tenant name and phone are required" });

                var result = _tenantService.AddTenant(model);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to save tenant, please try again" });

                return Ok(new { success = true, data = result, message = "Tenant added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant with TenantName: {TenantName}", model?.TenantName);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
