using PropHubAPI.Models.Apps.PropHUB;

namespace PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IFeedback
    {
        bool SubmitFeedback(FeedbackData model);
        IEnumerable<FeedbackView> GetFeedbackList(string? status, string? category);
        bool UpdateFeedbackStatus(UpdateFeedbackStatusRequest request);
    }
}
