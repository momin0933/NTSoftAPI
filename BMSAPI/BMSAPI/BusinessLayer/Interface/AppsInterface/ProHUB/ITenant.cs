using BMSAPI.BusinessLayer.TenantService;
using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface ITenant
    {
        bool AddTenant(TenantData model);
        IEnumerable<TenantFullView> GetTenantList(string phone);
    }
}
