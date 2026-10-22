using BMSAPI.Models.Common;
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IMenu : ICommonMethod<UserRoles>
    {
        List<RptMenu> GetAllMenuListWithUserRole(string UserRole, string projectName);
        List<RptMenu> GetAllMenuListWithUserRoleAndCompany(string UserRole, int companyId);
        List<RptMenu> GetAllMenuListWithUserRoleComapnyModule(string UserRole, int companyId, int moduleId);
        List<RptMenu> GetAllMenuList(int ModuleId);
        List<Project> GetAllProjectList();
        List<UserRoles> GetAllRoleList(int companyId);
        bool PostRoleAccess(int CompanyId, List<RoleAccess> itemList);
        List<RptMenu> GetMenuList(int moduleId);
        List<RptMenu> GetMenuById(int menuId);
        int Add(MenuViewModel menuVM);
        int Update(MenuViewModel rptMenu);
        int UpdateCompanyWiseMenu(CompanyWiseMenuUpdate menu);
    }
}
