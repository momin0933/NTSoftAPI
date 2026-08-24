using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IProperty _propertyService;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(IProperty propertyService, ILogger<PropertyController> logger)
        {
            _propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/AddProperty")]
        public IActionResult AddProperty([FromBody] Property model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Property data is required" });

                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Address))
                    return BadRequest(new { success = false, message = "Property name and address are required" });

                var result = _propertyService.AddProperty(model);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to save property, please try again" });

                return Ok(new { success = true, data = result, message = "Property saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving property with Name: {Name}", model?.Name);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("api/GetPropertyList")]
        public IActionResult GetPropertyList()
        {
            try
            {
                var list = _propertyService.GetPropertyList();
                return Ok(new { success = true, data = list, message = "Property list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property list");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/AddPropertyDetails")]
        public IActionResult AddPropertyDetails([FromBody] PropertyDetails model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Property details data is required" });

                if (model.PropertyId == null || model.PropertyId <= 0)
                    return BadRequest(new { success = false, message = "A valid PropertyId is required" });

                if (string.IsNullOrWhiteSpace(model.FlatName) || string.IsNullOrWhiteSpace(model.Floor))
                    return BadRequest(new { success = false, message = "Flat name and floor are required" });

                var result = _propertyService.AddPropertyDetails(model);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to save property details, please try again" });

                return Ok(new { success = true, data = result, message = "Property details saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving property details for PropertyId: {PropertyId}", model?.PropertyId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("api/GetPropertyDetailsList")]
        public IActionResult GetPropertyDetailsList(int propertyId)
        {
            try
            {
                if (propertyId <= 0)
                    return BadRequest(new { success = false, message = "A valid propertyId is required" });

                var list = _propertyService.GetPropertyDetailsList(propertyId);
                return Ok(new { success = true, data = list, message = "Property details list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property details list for PropertyId: {PropertyId}", propertyId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("api/GetMyPropertyList")]
        public IActionResult GetMyPropertyList(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return BadRequest(new { success = false, message = "A valid phone number is required" });

                var list = _propertyService.GetMyPropertyList(phone);
                return Ok(new { success = true, data = list, message = "My property list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving my property list for Phone: {Phone}", phone);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetPropertyDetailsFullList")]
        public IActionResult GetPropertyDetailsFullList(int propertyId)
        {
            try
            {
                if (propertyId <= 0)
                    return BadRequest(new { success = false, message = "A valid propertyId is required" });

                var list = _propertyService.GetPropertyDetailsFullList(propertyId);
                return Ok(new { success = true, data = list, message = "Full property details retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving full property details for PropertyId: {PropertyId}", propertyId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("api/ToggleProperty")]
        public IActionResult ToggleProperty([FromBody] TogglePropertyStatusRequest request)
        {
            try
            {
                if (request == null || request.PropertyId <= 0 || string.IsNullOrWhiteSpace(request.Phone))
                    return BadRequest(new { success = false, message = "A valid PropertyId and Phone are required" });

                var result = _propertyService.ToggleActiveStatus(
                    request.PropertyId,
                    request.IsActive,
                    request.Phone,
                    request.Phone
                );

                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to update property status, please try again" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = request.IsActive ? "Property activated successfully" : "Property deactivated successfully",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling property status for PropertyId: {PropertyId}", request?.PropertyId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
