using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class ComplainController : ControllerBase
    {
        private readonly IComplain _complainService;
        private readonly ILogger<ComplainController> _logger;

        public ComplainController(IComplain complainService, ILogger<ComplainController> logger)
        {
            _complainService = complainService ?? throw new ArgumentNullException(nameof(complainService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/AddComplain")]
        public IActionResult AddComplain([FromBody] AddComplainRequest request)
        {
            try
            {
                if (request == null || request.TenantId <= 0 || string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Description))
                    return BadRequest(new { success = false, message = "TenantId, Subject and Description are required" });

                var newId = _complainService.AddComplain(request);
                if (newId <= 0)
                    return StatusCode(500, new { success = false, message = "Failed to submit complaint, please try again" });

                return Ok(new { success = true, data = newId, message = "Complaint submitted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding complain");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetComplainListForAdmin")]
        public IActionResult GetComplainListForAdmin(int uId)
        {
            try
            {
                if (uId <= 0)
                    return BadRequest(new { success = false, message = "A valid uId is required" });

                var list = _complainService.GetComplainListForAdmin(uId);
                return Ok(new { success = true, data = list, message = "Complaint list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complain list for UId: {UId}", uId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetComplainListForTenant")]
        public IActionResult GetComplainListForTenant(int tenantId, int uId)
        {
            try
            {
                if (tenantId <= 0)
                    return BadRequest(new { success = false, message = "A valid tenantId is required" });

                var list = _complainService.GetComplainListForTenant(tenantId, uId);
                return Ok(new { success = true, data = list, message = "Complaint list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complain list for TenantId: {TenantId}", tenantId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/MarkComplainAsRead")]
        public IActionResult MarkComplainAsRead([FromBody] MarkComplainReadRequest request)
        {
            try
            {
                if (request == null || request.ComplainId <= 0 || request.UId <= 0)
                    return BadRequest(new { success = false, message = "A valid ComplainId and UId are required" });

                var result = _complainService.MarkAsRead(request.ComplainId, request.UId, request.EntryBy);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to mark complaint as read" });

                return Ok(new { success = true, data = result, message = "Complaint marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking complain as read");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetUnreadComplainCount")]
        public IActionResult GetUnreadComplainCount(int uId)
        {
            try
            {
                if (uId <= 0)
                    return BadRequest(new { success = false, message = "A valid uId is required" });

                var count = _complainService.GetUnreadCount(uId);
                return Ok(new { success = true, data = count, message = "Unread count retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread complain count for UId: {UId}", uId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/UpdateComplainStatus")]
        public IActionResult UpdateComplainStatus([FromBody] UpdateComplainStatusRequest request)
        {
            try
            {
                if (request == null || request.ComplainId <= 0 || request.UId <= 0 || string.IsNullOrWhiteSpace(request.Status))
                    return BadRequest(new { success = false, message = "A valid ComplainId, UId and Status are required" });

                var result = _complainService.UpdateStatus(request.ComplainId, request.UId, request.Status, request.Remarks, request.EntryBy);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to update complaint status" });

                return Ok(new { success = true, data = result, message = "Complaint status updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complain status");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
