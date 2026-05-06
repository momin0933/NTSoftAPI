using CentralAPI.Models.ReportModels;

namespace CentralAPI.BusinessLayer.Interface
{
    public interface IMenu 
    {
        List<RptMenu> GetAllMenuListWithUserRole(string UserRole, string projectName);
    }
}
