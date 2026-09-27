using PropHubAPI.BusinessLayer.TenantService;
using PropHubAPI.Models.Apps.PropHUB;

namespace PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface ITenant
    {
        bool AddTenant(TenantData model);
        IEnumerable<TenantFullView> GetTenantList(string phone, int uId);
        TenantFullView? GetMyTenancy(string tenantPhone);
    }
}