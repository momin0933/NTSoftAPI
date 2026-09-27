using PropHubAPI.Models;

namespace PropHubAPI.BusinessLayer.Interface
{
    public interface IMasterChild
    {
        IEnumerable<MasterChildItem> GetChildList(string? accessKey, int? parentId);
    }
}
