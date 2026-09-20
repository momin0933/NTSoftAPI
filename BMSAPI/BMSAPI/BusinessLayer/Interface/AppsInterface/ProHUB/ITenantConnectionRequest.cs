using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface ITenantConnectionRequest
    {
        int SendRequest(SendConnectionRequestBody body);
        IEnumerable<ConnectionRequestItem> GetForLandlord(int landlordUId, string landlordMobile);
        IEnumerable<ConnectionRequestItem> GetForTenant(int tenantUId);
        bool Accept(int requestId, string entryBy);
        bool Reject(int requestId, string entryBy);
        AutoMatchResult? CheckAutoMatch(string tenantPhone);
    }
}
