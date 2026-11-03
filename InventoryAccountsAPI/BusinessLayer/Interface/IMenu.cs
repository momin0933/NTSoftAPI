
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
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

        public int Add(MenuViewModel menuVM);

        public int Update(MenuViewModel rptMenu);

        public int UpdateCompanyWiseMenu(CompanyWiseMenuUpdate menu);
    }
}
