using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface ITenantHome
    {
        int AddTenantHome(AddTenantHomeRequest request);
        TenantHomeItem? GetMyCurrentHome(int uId);
        IEnumerable<TenantHomeItem> GetMyHomeHistory(int uId);
        bool UpdateConnection(int tenantHomeId, int uId, string connectionStatus, int? linkedTenancyId, string entryBy);
    }
}
