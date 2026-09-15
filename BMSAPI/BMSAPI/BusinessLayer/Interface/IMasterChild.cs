using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IMasterChild
    {
        IEnumerable<MasterChildItem> GetChildList(string? accessKey, int? parentId);
    }
}
