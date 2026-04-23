namespace NTSoftCentralAPI.BusinessLayer.TenantService
{
    public interface ITenantProvider
    {
        string GetConnectionString();
        string GetTenantId();
    }
}
