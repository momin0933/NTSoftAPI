using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class TenantHomeController : ControllerBase
    {
        private readonly ITenantHome _tenantHomeService;
        private readonly ILogger<TenantHomeController> _logger;

        public TenantHomeController(ITenantHome tenantHomeService, ILogger<TenantHomeController> logger)
        {
            _tenantHomeService = tenantHomeService ?? throw new ArgumentNullException(nameof(tenantHomeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/AddTenantHome")]
        public IActionResult AddTenantHome([FromBody] AddTenantHomeRequest request)
        {
            try
            {
                if (request == null || request.UId <= 0 || string.IsNullOrWhiteSpace(request.PropertyNameText) || string.IsNullOrWhiteSpace(request.TenantName))
                    return BadRequest(new { success = false, message = "UId, PropertyNameText and TenantName are required" });

                var newId = _tenantHomeService.AddTenantHome(request);
                if (newId <= 0)
                    return StatusCode(500, new { success = false, message = "Failed to save home, please try again" });

                return Ok(new { success = true, data = newId, message = "Home saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant home");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetMyCurrentHome")]
        public IActionResult GetMyCurrentHome(int uId)
        {
            try
            {
                if (uId <= 0)
                    return BadRequest(new { success = false, message = "A valid uId is required" });

                var home = _tenantHomeService.GetMyCurrentHome(uId);
                return Ok(new { success = true, data = home, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current home for UId: {UId}", uId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetMyHomeHistory")]
        public IActionResult GetMyHomeHistory(int uId)
        {
            try
            {
                if (uId <= 0)
                    return BadRequest(new { success = false, message = "A valid uId is required" });

                var list = _tenantHomeService.GetMyHomeHistory(uId);
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving home history for UId: {UId}", uId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/UpdateTenantHomeConnection")]
        public IActionResult UpdateTenantHomeConnection([FromBody] UpdateTenantHomeConnectionRequest request)
        {
            try
            {
                if (request == null || request.TenantHomeId <= 0 || request.UId <= 0)
                    return BadRequest(new { success = false, message = "A valid TenantHomeId and UId are required" });

                var result = _tenantHomeService.UpdateConnection(request);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to update connection" });

                return Ok(new { success = true, data = result, message = "Connection updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant home connection");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/DeactivateMyHome")]
        public IActionResult DeactivateMyHome([FromBody] DeactivateHomeRequest request)
        {
            try
            {
                if (request == null || request.TenantHomeId <= 0 || request.UId <= 0)
                    return BadRequest(new { success = false, message = "A valid TenantHomeId and UId are required" });

                var result = _tenantHomeService.DeactivateMyHome(request.TenantHomeId, request.UId, request.EntryBy);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to deactivate home" });

                return Ok(new { success = true, data = result, message = "Home deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating home");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/AddTenantHomeFromTenancy")]
        public IActionResult AddTenantHomeFromTenancy([FromBody] AddTenantHomeFromTenancyRequest request)
        {
            try
            {
                if (request == null || request.UId <= 0 || request.SourceTenantId <= 0)
                    return BadRequest(new { success = false, message = "UId and SourceTenantId are required" });

                var newId = _tenantHomeService.AddTenantHomeFromTenancy(request);
                if (newId <= 0)
                    return StatusCode(500, new { success = false, message = "Failed to add to profile, please try again" });

                return Ok(new { success = true, data = newId, message = "Added to your profile" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant home from tenancy");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
