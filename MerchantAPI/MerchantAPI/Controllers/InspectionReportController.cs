using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.Models;
using MerchantAPI.Models.Inspections;

namespace MerchantAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InspectionReportController : ControllerBase
    {
        private readonly IInspectionReportManager _inspectionService;
        private readonly ILogger<InspectionReportController> _logger;

        public InspectionReportController(IInspectionReportManager inspectionService, ILogger<InspectionReportController> logger)
        {
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // user name from the bearer token
        private string? CurrentUser =>
            User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? User.Identity?.Name;

        /// <summary>
        /// Get all active inspection reports (paginated, optional date range and search)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 50, DateTime? fromDate = null, DateTime? toDate = null, string? search = null)
        {
            try
            {
                var (data, totalCount) = await _inspectionService.GetAllAsync(pageNumber, pageSize, fromDate, toDate, search);
                return Ok(new { success = true, data, totalCount, message = "Inspection reports retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inspection reports");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get inspection report by ID (with all rows and photos)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var report = await _inspectionService.GetByIdAsync(id);
                if (report == null)
                    return NotFound(new { success = false, message = "Inspection report not found" });

                return Ok(new { success = true, data = report, message = "Inspection report retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving inspection report with id: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Add new inspection report
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] InspectionReport report)
        {
            try
            {
                if (report == null)
                    return BadRequest(new { success = false, message = "Inspection report data is required" });

                var id = await _inspectionService.AddAsync(report, CurrentUser ?? report.EntryBy);
                return CreatedAtAction(nameof(GetById), new { id }, new { success = true, data = new { id }, message = "Inspection report added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inspection report");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update inspection report
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] InspectionReport report)
        {
            try
            {
                if (report == null || report.Id <= 0)
                    return BadRequest(new { success = false, message = "Valid inspection report data is required" });

                var result = await _inspectionService.UpdateAsync(report, CurrentUser ?? report.UpdateBy);
                if (!result)
                    return NotFound(new { success = false, message = "Inspection report not found" });

                return Ok(new { success = true, data = new { id = report.Id }, message = "Inspection report updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inspection report");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete inspection report (soft delete)
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Valid inspection report ID is required" });

                var result = await _inspectionService.DeleteAsync(id, CurrentUser);
                if (!result)
                    return NotFound(new { success = false, message = "Inspection report not found" });

                return Ok(new { success = true, data = result, message = "Inspection report deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting inspection report: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}