using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class TenantConnectionRequestController : ControllerBase
    {
        private readonly ITenantConnectionRequest _service;
        private readonly ILogger<TenantConnectionRequestController> _logger;

        public TenantConnectionRequestController(ITenantConnectionRequest service, ILogger<TenantConnectionRequestController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("api/CheckAutoMatch")]
        public IActionResult CheckAutoMatch(string tenantPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantPhone))
                    return BadRequest(new { success = false, message = "tenantPhone is required" });

                var match = _service.CheckAutoMatch(tenantPhone);
                return Ok(new { success = true, data = match, message = "Checked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking auto-match");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/SendConnectionRequest")]
        public IActionResult SendConnectionRequest([FromBody] SendConnectionRequestBody body)
        {
            try
            {
                if (body == null || body.TenantUId <= 0 || body.TenantHomeId <= 0)
                    return BadRequest(new { success = false, message = "TenantUId and TenantHomeId are required" });

                var newId = _service.SendRequest(body);
                if (newId <= 0)
                    return StatusCode(500, new { success = false, message = "Failed to send request" });

                return Ok(new { success = true, data = newId, message = "Request sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending connection request");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetConnectionRequestsForLandlord")]
        public IActionResult GetConnectionRequestsForLandlord(int landlordUId, string landlordMobile)
        {
            try
            {
                var list = _service.GetForLandlord(landlordUId, landlordMobile);
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving requests for landlord");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetConnectionRequestsForTenant")]
        public IActionResult GetConnectionRequestsForTenant(int tenantUId)
        {
            try
            {
                if (tenantUId <= 0)
                    return BadRequest(new { success = false, message = "A valid tenantUId is required" });

                var list = _service.GetForTenant(tenantUId);
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving requests for tenant");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/AcceptConnectionRequest")]
        public IActionResult AcceptConnectionRequest([FromBody] RespondConnectionRequestBody body)
        {
            try
            {
                if (body == null || body.RequestId <= 0 || body.LandlordUId <= 0)
                    return BadRequest(new { success = false, message = "A valid RequestId and LandlordUId are required" });

                var result = _service.Accept(body.RequestId, body.LandlordUId, body.EntryBy);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to accept request" });

                return Ok(new { success = true, data = result, message = "Request accepted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting connection request");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("api/RejectConnectionRequest")]
        public IActionResult RejectConnectionRequest([FromBody] RespondConnectionRequestBody body)
        {
            try
            {
                if (body == null || body.RequestId <= 0)
                    return BadRequest(new { success = false, message = "A valid RequestId is required" });

                var result = _service.Reject(body.RequestId, body.EntryBy);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to reject request" });

                return Ok(new { success = true, data = result, message = "Request rejected" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting connection request");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
