using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.Models.Inspections;
using MerchantAPI.BusinessLayer.Manager;

namespace MerchantAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InspectionSiteController : ControllerBase
    {
        private readonly IInspectionSite _siteService;
        private readonly ILogger<InspectionSiteController> _logger;

        public InspectionSiteController(IInspectionSite siteService, ILogger<InspectionSiteController> logger)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // user name from the bearer token
        private string? CurrentUser =>
            User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? User.Identity?.Name;

        /// <summary>All active sites (for the Site name dropdown); optional search text.</summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(string? search = null)
        {
            try
            {
                var data = await _siteService.GetAllAsync(search);
                return Ok(new { success = true, data, message = "Inspection sites retrieved successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inspection sites");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var site = await _siteService.GetByIdAsync(id);
                if (site == null)
                    return NotFound(new { success = false, message = "Inspection site not found" });

                return Ok(new { success = true, data = site, message = "Inspection site retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving inspection site {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] InspectionSite site)
        {
            try
            {
                if (site == null)
                    return BadRequest(new { success = false, message = "Site data is required" });

                var id = await _siteService.AddAsync(site, CurrentUser ?? site.EntryBy);
                return CreatedAtAction(nameof(GetById), new { id }, new { success = true, data = new { id }, message = "Inspection site added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inspection site");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] InspectionSite site)
        {
            try
            {
                if (site == null || site.Id <= 0)
                    return BadRequest(new { success = false, message = "Valid site data is required" });

                var result = await _siteService.UpdateAsync(site, CurrentUser ?? site.UpdateBy);
                if (!result)
                    return NotFound(new { success = false, message = "Inspection site not found" });

                return Ok(new { success = true, data = new { id = site.Id }, message = "Inspection site updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inspection site");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Valid site ID is required" });

                var result = await _siteService.DeleteAsync(id, CurrentUser);
                if (!result)
                    return NotFound(new { success = false, message = "Inspection site not found" });

                return Ok(new { success = true, data = result, message = "Inspection site deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting inspection site: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}