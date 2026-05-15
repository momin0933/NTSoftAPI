using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.Models;

namespace MerchantAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DevelopmentController : ControllerBase
    {
        private readonly IDevelopmentManager _developmentService;
        private readonly ILogger<DevelopmentController> _logger;

        public DevelopmentController(IDevelopmentManager developmentService, ILogger<DevelopmentController> logger)
        {
            _developmentService = developmentService ?? throw new ArgumentNullException(nameof(developmentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all developments (paginated)
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAllDevelopments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                    return BadRequest(new { success = false, message = "PageNumber and PageSize must be greater than 0" });

                var developments = _developmentService.GetAllDevelopments(pageNumber, pageSize);
                return Ok(new { success = true, data = developments, message = "Developments retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all developments");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get development by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetDevelopmentById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Valid development ID is required" });

                var development = _developmentService.GetDevelopmentById(id);
                if (development == null)
                    return NotFound(new { success = false, message = "Development not found" });

                return Ok(new { success = true, data = development, message = "Development retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving development with id: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get developments by buyer ID
        /// </summary>
        [HttpGet("buyer/{buyerId}")]
        public IActionResult GetDevelopmentsByBuyer(int buyerId)
        {
            try
            {
                if (buyerId <= 0)
                    return BadRequest(new { success = false, message = "Valid buyer ID is required" });

                var developments = _developmentService.GetDevelopmentsByBuyer(buyerId);
                return Ok(new { success = true, data = developments, message = "Developments retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving developments for buyer: {buyerId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get developments by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public IActionResult GetDevelopmentsByCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    return BadRequest(new { success = false, message = "Valid customer ID is required" });

                var developments = _developmentService.GetDevelopmentsByCustomer(customerId);
                return Ok(new { success = true, data = developments, message = "Developments retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving developments for customer: {customerId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get developments by factory ID
        /// </summary>
        [HttpGet("factory/{factoryId}")]
        public IActionResult GetDevelopmentsByFactory(int factoryId)
        {
            try
            {
                if (factoryId <= 0)
                    return BadRequest(new { success = false, message = "Valid factory ID is required" });

                var developments = _developmentService.GetDevelopmentsByFactory(factoryId);
                return Ok(new { success = true, data = developments, message = "Developments retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving developments for factory: {factoryId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get developments by date range (InqDate)
        /// </summary>
        [HttpGet("date-range")]
        public IActionResult GetDevelopmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                    return BadRequest(new { success = false, message = "Valid start and end dates are required" });

                if (startDate >= endDate)
                    return BadRequest(new { success = false, message = "Start date must be before end date" });

                var developments = _developmentService.GetDevelopmentsByDateRange(startDate, endDate);
                return Ok(new { success = true, data = developments, message = "Developments retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving developments for date range: {startDate} - {endDate}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Add new development
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddDevelopment([FromBody] Development development)
        {
            try
            {
                if (development == null)
                    return BadRequest(new { success = false, message = "Development data is required" });

                var result = await _developmentService.AddDevelopmentAsync(development);
                return CreatedAtAction(
                    nameof(GetDevelopmentById),
                    new { id = result },
                    new { success = true, data = result, message = "Development added successfully" }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding development");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing development
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDevelopment([FromBody] Development development)
        {
            try
            {
                if (development == null || development.Id <= 0)
                    return BadRequest(new { success = false, message = "Valid development data is required" });

                var result = await _developmentService.UpdateDevelopmentAsync(development);
                if (!result)
                    return NotFound(new { success = false, message = "Development not found or not updated" });

                return Ok(new { success = true, data = result, message = "Development updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating development with id: {development?.Id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete development (soft delete)
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDevelopment(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Valid development ID is required" });

                var result = await _developmentService.DeleteDevelopmentAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Development not found or already deleted" });

                return Ok(new { success = true, data = result, message = "Development deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting development with id: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}