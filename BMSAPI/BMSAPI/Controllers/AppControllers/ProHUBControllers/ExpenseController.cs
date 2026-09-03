using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpense _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpense expenseService, ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }
        [HttpPost("api/AddExpense")]
        public IActionResult AddExpense([FromBody] AddExpenseRequest request)
        {
            try
            {
                if (request == null
                    || string.IsNullOrWhiteSpace(request.Phone)
                    || string.IsNullOrWhiteSpace(request.ExpenseName)
                    || string.IsNullOrWhiteSpace(request.ExpenseType)
                    || request.ExpenseAmount == null
                    || request.ExpenseAmount <= 0)
                {
                    return BadRequest(new { success = false, message = "Phone, ExpenseName, ExpenseType, and a valid ExpenseAmount are required" });
                }

                var newId = _expenseService.AddExpense(request);
                return Ok(new { success = true, data = newId, message = "Expense saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding expense");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetExpenseList")]
        public IActionResult GetExpenseList(string phone, int? expenseMonth = null, int? expenseYear = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return BadRequest(new { success = false, message = "Phone is required" });
                }

                var list = _expenseService.GetExpenseList(phone, expenseMonth, expenseYear);
                return Ok(new { success = true, data = list, message = "Expense list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense list");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetExpenseSourceOptions")]
        public IActionResult GetExpenseSourceOptions(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return BadRequest(new { success = false, message = "Phone is required" });
                }

                var options = _expenseService.GetExpenseSourceOptions(phone);
                return Ok(new { success = true, data = options });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense source options");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/DeleteExpense")]
        public IActionResult DeleteExpense(string phone, int expenseId, string updateBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) || expenseId <= 0)
                {
                    return BadRequest(new { success = false, message = "Phone and ExpenseId are required" });
                }

                var deleted = _expenseService.DeleteExpense(phone, expenseId, updateBy);
                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Expense not found." });
                }

                return Ok(new { success = true, data = true, message = "Expense deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
