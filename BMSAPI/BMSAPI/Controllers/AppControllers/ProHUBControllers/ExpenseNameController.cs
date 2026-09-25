using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class ExpenseNameController : ControllerBase
    {
        private readonly IExpenseName _service;
        private readonly ILogger<ExpenseNameController> _logger;

        public ExpenseNameController(IExpenseName service, ILogger<ExpenseNameController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Universal names + the logged-in user's own names.
        [HttpGet("api/GetExpenseNameList")]
        public IActionResult GetExpenseNameList(int uId)
        {
            try
            {
                if (uId <= 0)
                    return BadRequest(new { success = false, message = "A valid uId is required" });

                var list = _service.GetList(uId);
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense names");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Adds a custom name for this user. If the same name already exists
        // (universal or the user's own), nothing is created and the existing
        // Id comes back with duplicate = true, so the app can just select it.
        [HttpPost("api/AddExpenseName")]
        public IActionResult AddExpenseName([FromBody] AddExpenseNameRequest body)
        {
            try
            {
                if (body == null || body.UId <= 0 || string.IsNullOrWhiteSpace(body.Name))
                    return BadRequest(new { success = false, message = "UId and Name are required" });

                var result = _service.Add(body);

                if (result.Status == "Duplicate")
                    return Ok(new
                    {
                        success = true,
                        data = new { id = result.Id, duplicate = true },
                        message = "This expense name already exists"
                    });

                if (result.Status != "Created" || result.Id == null)
                    return StatusCode(500, new { success = false, message = "Failed to add expense name" });

                return Ok(new
                {
                    success = true,
                    data = new { id = result.Id, duplicate = false },
                    message = "Expense name added successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding expense name");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Only the owner can edit; universal names can't be edited here.
        [HttpPost("api/UpdateExpenseName")]
        public IActionResult UpdateExpenseName([FromBody] UpdateExpenseNameRequest body)
        {
            try
            {
                if (body == null || body.Id <= 0 || body.UId <= 0 || string.IsNullOrWhiteSpace(body.Name))
                    return BadRequest(new { success = false, message = "Id, UId and Name are required" });

                var result = _service.Update(body);

                if (result.Status == "Duplicate")
                    return Conflict(new { success = false, data = new { id = result.Id }, message = "This expense name already exists" });

                if (result.Status == "NotFound")
                    return NotFound(new { success = false, message = "Expense name not found or not yours to edit" });

                if (result.Status != "Updated")
                    return StatusCode(500, new { success = false, message = "Failed to update expense name" });

                return Ok(new { success = true, data = new { id = result.Id }, message = "Expense name updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense name");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Soft delete. Only the owner can remove; universal names can't be removed here.
        [HttpPost("api/DeleteExpenseName")]
        public IActionResult DeleteExpenseName([FromBody] DeleteExpenseNameRequest body)
        {
            try
            {
                if (body == null || body.Id <= 0 || body.UId <= 0)
                    return BadRequest(new { success = false, message = "Id and UId are required" });

                var deleted = _service.Delete(body.Id, body.UId, body.EntryBy);
                if (!deleted)
                    return NotFound(new { success = false, message = "Expense name not found or not yours to delete" });

                return Ok(new { success = true, data = true, message = "Expense name removed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense name");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
