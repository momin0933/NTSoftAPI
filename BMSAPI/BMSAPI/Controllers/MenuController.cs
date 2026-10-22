using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models.Common;
using BMSAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenu _ServiceManager;

        public MenuController(IMenu menu)
        {
            _ServiceManager = menu;
        }

        [Route("api/Menu/GetAllProjectList")]
        [HttpGet]
        public List<Project> GetAllProjectList() => _ServiceManager.GetAllProjectList();

        [Route("api/Menu/GetAllRoleList")]
        [HttpGet]
        public List<UserRoles> GetAllRoleList(int companyId) => _ServiceManager.GetAllRoleList(companyId);

        [Route("api/Menu/GetAllMenuRoleWise")]
        [HttpGet]
        public List<RptMenu> GetAllMenuWithUserRole(string UserRole, string projectName)
            => _ServiceManager.GetAllMenuListWithUserRole(UserRole, projectName);

        [Route("api/Menu/GetAllMenuRoleAndCompanyWise")]
        [HttpGet]
        public List<RptMenu> GetAllMenuWithUserRoleAndCompany(string UserRole, int companyId)
            => _ServiceManager.GetAllMenuListWithUserRoleAndCompany(UserRole, companyId);

        [Route("api/Menu/GetAllMenuList")]
        [HttpGet]
        public List<RptMenu> GetAllMenuList(int ModuleId) => _ServiceManager.GetAllMenuList(ModuleId);

        [Route("api/Menu/AddRoleAccess")]
        [HttpPost]
        public void AddRoleAccess([FromQuery] int CompanyId, [FromBody] List<RoleAccess> ItemList)
            => _ServiceManager.PostRoleAccess(CompanyId, ItemList);

        [Route("api/Menu/Role")]
        [HttpPost]
        public int Post([FromBody] UserRoles entity) => _ServiceManager.Add(entity);

        [Route("api/Menu/Role")]
        [HttpPut]
        public int Put([FromBody] UserRoles entity) => _ServiceManager.Update(entity);

        [Route("api/Menu/GetAllMenu")]
        [HttpGet]
        public List<RptMenu> GetAllMenu(int moduleId) => _ServiceManager.GetMenuList(moduleId);

        [Route("api/Menu/AddMenu")]
        [HttpPost]
        public int Post([FromBody] MenuViewModel menuVM) => _ServiceManager.Add(menuVM);

        [Route("api/Menu/GetMenuById")]
        [HttpGet]
        public List<RptMenu> GetMenuById(int menuId) => _ServiceManager.GetMenuById(menuId);

        [Route("api/Menu/UpdateMenu")]
        [HttpPut]
        public int Update([FromBody] MenuViewModel rptMenu) => _ServiceManager.Update(rptMenu);

        [Route("api/Menu/UpdateCompanyWiseMenu")]
        [HttpPost]
        public int UpdateCompanyWiseMenu([FromBody] CompanyWiseMenuUpdate menu)
            => _ServiceManager.UpdateCompanyWiseMenu(menu);

        [Route("api/Menu/GetAllMenuRoleCompanyModule")]
        [HttpGet]
        public List<RptMenu> GetAllMenuRoleCompanyModule(string UserRole, int companyId, int moduleId)
            => _ServiceManager.GetAllMenuListWithUserRoleComapnyModule(UserRole, companyId, moduleId);
    }
}
