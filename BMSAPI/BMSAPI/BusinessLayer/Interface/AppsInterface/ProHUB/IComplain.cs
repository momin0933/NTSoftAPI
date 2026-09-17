using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IComplain
    {
        int AddComplain(AddComplainRequest request);
        IEnumerable<ComplainView> GetComplainListForAdmin(int uId);
        IEnumerable<ComplainView> GetComplainListForTenant(int tenantId, int uId);
        bool MarkAsRead(int complainId, int uId, string entryBy);
        int GetUnreadCount(int uId);
        bool UpdateStatus(int complainId, int uId, string status, string remarks, string entryBy);
    }
}
