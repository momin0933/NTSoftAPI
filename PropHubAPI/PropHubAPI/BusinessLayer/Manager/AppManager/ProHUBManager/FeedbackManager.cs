using PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using PropHubAPI.BusinessLayer.Service;
using PropHubAPI.Models.Apps.PropHUB;
using Dapper;

namespace PropHubAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class FeedbackManager : IFeedback
    {
        private readonly ILogger<FeedbackManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Feedback";

        public FeedbackManager(IDapperService dapperService, ILogger<FeedbackManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool SubmitFeedback(FeedbackData model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@UId", model.UId);
                p.Add("@Phone", model.Phone);
                p.Add("@UserRole", model.UserRole);
                p.Add("@Rating", model.Rating);
                p.Add("@Category", model.Category);
                p.Add("@Message", model.Message);
                p.Add("@Email", model.Email);
                p.Add("@Platform", model.Platform);
                p.Add("@AppVersion", model.AppVersion);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Submit feedback failed for UId: {UId}", model.UId);
                    return false;
                }

                _logger.LogInformation("Feedback {Id} submitted by UId {UId}, rating {Rating}",
                    (int)result.Id, model.UId, model.Rating);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback for UId: {UId}", model.UId);
                throw;
            }
        }

        public IEnumerable<FeedbackView> GetFeedbackList(string? status, string? category)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Status", string.IsNullOrWhiteSpace(status) ? null : status);
                p.Add("@Category", string.IsNullOrWhiteSpace(category) ? null : category);

                return _IDapperService.GetAllBySP<FeedbackView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback list");
                throw;
            }
        }

        public bool UpdateFeedbackStatus(UpdateFeedbackStatusRequest request)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Id", request.Id);
                p.Add("@Status", request.Status);
                p.Add("@AdminNote", request.AdminNote);
                p.Add("@EntryBy", request.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return (int)result.AffectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback {Id}", request.Id);
                throw;
            }
        }
    }
}