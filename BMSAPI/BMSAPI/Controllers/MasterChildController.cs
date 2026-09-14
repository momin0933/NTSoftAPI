using BMSAPI.BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [ApiController]
    public class MasterChildController : ControllerBase
    {
        private readonly IMasterChild _masterChildService;
        private readonly ILogger<MasterChildController> _logger;

        public MasterChildController(IMasterChild masterChildService, ILogger<MasterChildController> logger)
        {
            _masterChildService = masterChildService ?? throw new ArgumentNullException(nameof(masterChildService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("api/GetMasterChildList")]
        public IActionResult GetMasterChildList(string? accessKey, int? parentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessKey) && parentId == null)
                    return BadRequest(new { success = false, message = "Either accessKey or parentId is required" });

                var list = _masterChildService.GetChildList(accessKey, parentId);
                return Ok(new { success = true, data = list, message = "Retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving master child list for AccessKey: {AccessKey}", accessKey);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
