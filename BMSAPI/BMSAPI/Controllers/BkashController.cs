using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
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

        //[HttpPost("api/SaveBkashPayment")]
        //public IActionResult SaveBkashPayment([FromForm] BkashPaymentRequest request)
        //{
        //    try
        //    {
        //        var result = _bkashManager.SaveBkashPayment(request);
        //        if (!result)
        //        {
        //            return Unauthorized(new
        //            {
        //                success = false,
        //                message = "Invalid UserName or Password"
        //            });
        //        }

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Payment saved successfully"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving bKash payment");

        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}

        // ==============================
        // CONTROLLER
        // File: Controllers/BkashController.cs
        // ==============================

        [HttpPost("api/SaveBkashPayment")]
        public IActionResult SaveBkashPayment([FromForm] BkashPaymentRequest request)
        {
            try
            {
                var result = _bkashManager.SaveBkashPayment(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bKash payment");

                return StatusCode(500, new BkashBillPaymentResponse
                {
                    ErrorCode = "500",
                    ErrorMsg = ex.Message
                });
            }
        }


        //[HttpGet("api/GetBillByTrxId")]
        //public IActionResult GetBillByTrxId(string UserName, string Password, string TrxId)
        //{
        //    try
        //    {
        //        var Bill = _bkashManager.GetBillByTrxId(UserName, Password, TrxId);
        //        return Ok(new { success = true, data = Bill, message = "Bill retrieved successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving bill");
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        [HttpGet("api/GetBillByTrxId")]
        public IActionResult GetBillByTrxId(string UserName, string Password, string TrxId)
        {
            try
            {
                var result = _bkashManager.GetBillByTrxId(UserName, Password, TrxId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill");

                return StatusCode(500, new BkashBillPaymentResponse
                {
                    ErrorCode = "500",
                    ErrorMsg = ex.Message
                });
            }
        }

    }
}
