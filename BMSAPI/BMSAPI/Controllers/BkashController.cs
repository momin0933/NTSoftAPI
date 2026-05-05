using BMSAPI.BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    
    [ApiController]
    public class BkashController : ControllerBase
    {
        private readonly IBkashManager _bkashManager;
        private readonly ILogger<BkashController> _logger;

        public BkashController(IBkashManager bkashManager, ILogger<BkashController> logger)
        {
            _bkashManager = bkashManager ?? throw new ArgumentNullException(nameof(bkashManager ));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet("api/BillMonthWise")]
        public IActionResult GetBillMonthWise(string UserName, string Password, string FlatCode, string BillMonth)
        {
            try
            {
                var Bill = _bkashManager.GetBillMonthWise(UserName, Password, FlatCode, BillMonth);
                return Ok(new { success = true, data = Bill, message = "Bill retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
