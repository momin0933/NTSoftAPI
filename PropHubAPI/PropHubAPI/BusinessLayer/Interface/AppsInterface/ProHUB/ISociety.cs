using PropHubAPI.BusinessLayer.Manager.AppManager.ProHUBManager;

namespace PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface ISociety
    {
        IEnumerable<SocietyItem> GetSocietyList();
    }
}
