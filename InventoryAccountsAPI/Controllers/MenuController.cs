
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FTSoftAPI.Controllers
{

    [ApiController]
    public class MenuController : Controller
    {
        private readonly IMenu _ServiceManager;

        public MenuController(IMenu Menu)
        {
            _ServiceManager = Menu;
        }


        [Route("api/Menu/GetAllProjectList")]
        [HttpGet]
        public List<Project> GetAllProjectList()
        {
            try
            {
                List<Project> itemList = _ServiceManager.GetAllProjectList();
                return itemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/GetAllRoleList")]
        [HttpGet]
        public List<UserRoles> GetAllRoleList(int companyId)
        {
            try
            {
                List<UserRoles> itemList = _ServiceManager.GetAllRoleList(companyId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/GetAllMenuRoleWise")]
        [HttpGet]
        public List<RptMenu> GetAllMenuWithUserRole(string UserRole, string projectName)
        {
            try
            {

                List<RptMenu> itemList = _ServiceManager.GetAllMenuListWithUserRole(UserRole, projectName);
                return itemList;

            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/GetAllMenuRoleAndCompanyWise")]
        [HttpGet]
        public List<RptMenu> GetAllMenuWithUserRoleAndCompany(string UserRole, int companyId)
        {
            try
            {
                List<RptMenu> itemList = _ServiceManager.GetAllMenuListWithUserRoleAndCompany(UserRole, companyId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/GetAllMenuList")]
        [HttpGet]
        public List<RptMenu> GetAllMenuList(int ModuleId)
        {
            try
            {
                List<RptMenu> itemList = _ServiceManager.GetAllMenuList(ModuleId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }
        [Route("api/Menu/AddRoleAccess")]
        [HttpPost]
        public void AddRoleAccess(int CompanyId, List<RoleAccess> ItemList)
        {

            try
            {

                _ServiceManager.PostRoleAccess(CompanyId, ItemList);

            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/Role")]
        [HttpPost]
        public int Post([FromBody] UserRoles entity)
        {
            _ServiceManager.Add(entity);
            return entity.Id;
        }

        [Route("api/Menu/Role")]
        [HttpPut]
        public int Put([FromBody] UserRoles entity)
        {
            _ServiceManager.Update(entity);
            return entity.Id;
        }

        [Route("api/Menu/GetAllMenu")]
        [HttpGet]
        public List<RptMenu> GetAllMenu(int moduleId)
        {
            try
            {
                List<RptMenu> itemList = _ServiceManager.GetMenuList(moduleId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/Menu/AddMenu")]
        [HttpPost]
        public int Post([FromBody] MenuViewModel menuVM)
        {
            _ServiceManager.Add(menuVM);
            return menuVM.ModuleId;
        }
        [Route("api/Menu/GetMenuById")]
        [HttpGet]
        public List<RptMenu> GetMenuById(int menuId)
        {
            var menu = _ServiceManager.GetMenuById(menuId);
            return (menu);
        }
        [Route("api/Menu/UpdateMenu")]
        [HttpPut]
        public int Update([FromBody] MenuViewModel rptMenu)
        {
            _ServiceManager.Update(rptMenu);
            return rptMenu.ModuleId;
        }


        [Route("api/Menu/UpdateCompanyWiseMenu")]
        [HttpPost]
        public int UpdateCompanyWiseMenu([FromBody] CompanyWiseMenuUpdate menu)
        {
            _ServiceManager.UpdateCompanyWiseMenu(menu);
            return menu.MenuId;
        }

        [Route("api/Menu/GetAllMenuRoleCompanyModule")]
        [HttpGet]
        public List<RptMenu> GetAllMenuRoleCompanyModule(string UserRole, int companyId, int moduleId)
        {
            try
            {

                List<RptMenu> itemList = _ServiceManager.GetAllMenuListWithUserRoleComapnyModule(UserRole, companyId, moduleId);
                return itemList;

            }
            catch
            {
                throw;
            }
        }
    }
}