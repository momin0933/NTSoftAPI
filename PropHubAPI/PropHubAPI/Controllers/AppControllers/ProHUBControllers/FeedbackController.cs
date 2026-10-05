using PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using PropHubAPI.Models.Apps.PropHUB;
using Microsoft.AspNetCore.Mvc;

namespace PropHubAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private static readonly string[] AllowedCategories = { "bug", "idea", "praise", "other" };
        private static readonly string[] AllowedStatuses = { "New", "Reviewed", "Resolved" };

        private readonly IFeedback _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedback feedbackService, ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/SubmitFeedback")]
        public IActionResult SubmitFeedback([FromBody] FeedbackData model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Feedback data is required" });

                if (model.Rating == null || model.Rating < 1 || model.Rating > 5)
                    return BadRequest(new { success = false, message = "Please choose a rating from 1 to 5" });

                if (!string.IsNullOrWhiteSpace(model.Category) && !AllowedCategories.Contains(model.Category))
                    return BadRequest(new { success = false, message = "Invalid feedback category" });

                if (model.Message != null && model.Message.Length > 280)
                    return BadRequest(new { success = false, message = "Message must be 280 characters or fewer" });

                var result = _feedbackService.SubmitFeedback(model);
                if (!result)
                    return StatusCode(500, new { success = false, message = "Failed to send feedback, please try again" });

                return Ok(new { success = true, data = result, message = "Feedback received" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback for UId: {UId}", model?.UId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/GetFeedbackList")]
        public IActionResult GetFeedbackList(string? status, string? category)
        {
            try
            {
                var list = _feedbackService.GetFeedbackList(status, category);
                return Ok(new { success = true, data = list, message = "Feedback list retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback list");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/UpdateFeedbackStatus")]
        public IActionResult UpdateFeedbackStatus([FromBody] UpdateFeedbackStatusRequest request)
        {
            try
            {
                if (request == null || request.Id <= 0)
                    return BadRequest(new { success = false, message = "A valid feedback Id is required" });

                if (!string.IsNullOrWhiteSpace(request.Status) && !AllowedStatuses.Contains(request.Status))
                    return BadRequest(new { success = false, message = "Invalid status" });

                var result = _feedbackService.UpdateFeedbackStatus(request);
                if (!result)
                    return BadRequest(new { success = false, message = "Feedback not found" });

                return Ok(new { success = true, data = result, message = "Feedback updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback {Id}", request?.Id);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}