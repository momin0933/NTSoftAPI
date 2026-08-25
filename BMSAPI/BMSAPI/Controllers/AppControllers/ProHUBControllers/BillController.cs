using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBill _billService;
        private readonly ILogger<BillController> _logger;

        public BillController(IBill billService, ILogger<BillController> logger)
        {
            _billService = billService ?? throw new ArgumentNullException(nameof(billService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/GenerateBill")]
        public IActionResult GenerateBill([FromBody] GenerateBillRequest request)
        {
            try
            {
                if (request == null
                    || string.IsNullOrWhiteSpace(request.Phone)
                    || string.IsNullOrWhiteSpace(request.BillMonth)
                    || string.IsNullOrWhiteSpace(request.BillYear))
                {
                    return BadRequest(new { success = false, message = "Phone, BillMonth, and BillYear are required" });
                }

                var result = _billService.GenerateBill(request.Phone, request.BillMonth, request.BillYear, request.Phone);

                if (result.AlreadyGenerated)
                {
                    return Conflict(new
                    {
                        success = false,
                        message = $"Bills for {request.BillMonth} {request.BillYear} have already been generated.",
                    });
                }

                if (result.AffectedRows <= 0)
                {
                    return Ok(new
                    {
                        success = true,
                        data = 0,
                        message = "No active tenants found — no bills were generated.",
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result.AffectedRows,
                    message = $"{result.AffectedRows} bill(s) generated for {request.BillMonth} {request.BillYear}.",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bills for Phone: {Phone}", request?.Phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetBillList")]
        public IActionResult GetBillList(string phone, string billMonth, string billYear)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(billMonth) || string.IsNullOrWhiteSpace(billYear))
                    return BadRequest(new { success = false, message = "phone, billMonth, and billYear are required" });

                var list = _billService.GetBillList(phone, billMonth, billYear);
                return Ok(new { success = true, data = list, message = "Bill list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill list for Phone: {Phone}", phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/CheckBillExists")]
        public IActionResult CheckBillExists(string phone, string billMonth, string billYear)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(billMonth) || string.IsNullOrWhiteSpace(billYear))
                    return BadRequest(new { success = false, message = "phone, billMonth, and billYear are required" });

                var exists = _billService.CheckBillExists(phone, billMonth, billYear);
                return Ok(new { success = true, data = exists, message = "Checked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking bill existence for Phone: {Phone}", phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
