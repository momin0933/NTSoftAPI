using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace FTSoftAPI.Core.DataLayer.Repository
{
    public class MenuManager : IMenu
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        public MenuManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }        
        //public void Dispose()
        //{
        //    _dbContext.Dispose();
        //}
        public List<RptMenu> GetAllMenuListWithUserRole(string UserRole, string projectName)
        {
            try
            {
    
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 1);
                    p.Add("@UserRole", UserRole);
                    p.Add("@projectName", projectName);
                    List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).OrderBy(x => x.ModuleSorting).ToList();
                    return ItemList;
                
               
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<RptMenu> GetAllMenuList(int ModuleId)
        {
            try
            {
                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@ModuleId", ModuleId);
                List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Project> GetAllProjectList()
        {
            try
            {
                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);                
                List<Project> ItemList = _IDapperService.GetAllBySP<Project>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<UserRoles> GetAllRoleList(int companyId)
        {
            try
            {
                if(companyId != 0)
                {
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 4);
                    p.Add("@companyId", companyId);
                    List<UserRoles> ItemList = _IDapperService.GetAllBySP<UserRoles>(SP, p).ToList();
                    return ItemList;
                }
                else
                {
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 11);
                    p.Add("@companyId", companyId);
                    List<UserRoles> ItemList = _IDapperService.GetAllBySP<UserRoles>(SP, p).ToList();
                    return ItemList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool PostRoleAccess(int CompanyId, List<RoleAccess> itemList)
        {
            bool result = false;
            try
            {
                if(itemList.Count > 0)
                {
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 0);
                    p.Add("@CompanyId", CompanyId);
                    p.Add("@UserRole", itemList[0].Role);
                    int serverpost = _IDapperService.PostBySP(SP, p);
                    result = _IDapperService.PostBulkInsert<RoleAccess>(itemList, "tblMVCRoleAccess");
                }
               
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public List<UserRoles> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<UserRoles>().ToList();
            }
            catch
            {
                throw;
            }
        }

        public int Add(UserRoles entity)
        {
            try
            {                
                _ICommonService.Add<UserRoles>(entity);                
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Update(UserRoles entity)
        {
            try
            {
                _ICommonService.Update<UserRoles>(entity);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Remove(UserRoles entity)
        {
            try
            {
                //_ICommonService.RemoveById<Item>(entity.Id);                
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

		public List<RptMenu> GetMenuList(int moduleId)
		{
            try
            {
                if(moduleId == 0)
                {
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 7);
                    p.Add("moduleId", moduleId);
                    List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).ToList();
                    return ItemList;
                }
                else
                {
                    string SP = "SP_GETMENU";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 8);
                    p.Add("moduleId", moduleId);
                    List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).ToList();
                    return ItemList;
                }
               
            }
            catch (Exception ex)
            {
                return null;
            }         
		}

        public int Add(MenuViewModel menuVM)
        {
            var sqlQuery = @"INSERT INTO FashionTexLive.dbo.tblmvcmenu 
                    (ModuleId, ParentId, DisplayName, ActionResult, Controller, MenuIcon, IsActive, ProjectName,DisplayOrder)
                    VALUES (@ModuleId, @ParentId, @DisplayName, @ActionResult, @Controller, @MenuIcon, @IsActive, @ProjectName,@DisplayOrder);"
            ;
            var parentIdParam = new SqlParameter("@ParentId", menuVM.ParentId.HasValue && menuVM.ParentId != 0 ? (object)menuVM.ParentId : DBNull.Value);

            _dbContext.Database.ExecuteSqlRaw(sqlQuery,
                             new SqlParameter("@ModuleId", menuVM.ModuleId),
                             parentIdParam,
                             new SqlParameter("@DisplayName", menuVM.DisplayName),
                             new SqlParameter("@ActionResult", menuVM.ActionResult),
                             new SqlParameter("@Controller", menuVM.Controller),
                             new SqlParameter("@MenuIcon", menuVM.MenuIcon),
                             new SqlParameter("@IsActive", menuVM.IsActive),
                             new SqlParameter("@ProjectName", menuVM.ProjectName),
            new SqlParameter("@DisplayOrder", menuVM.DisplayOrder));


            return menuVM.ModuleId;
        }

        public List<RptMenu> GetMenuById(int menuId)
        {
            try
            {
                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 6);
                p.Add("@MenuId", menuId);
                List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int Update(MenuViewModel rptMenu )
        {
            var sqlQuery = @"UPDATE FashionTexLive.dbo.tblmvcmenu 
                     SET ModuleId = @ModuleId,
                         ParentId = @ParentId,
                         DisplayName = @DisplayName,
                         ActionResult = @ActionResult,
                         Controller = @Controller,
                         MenuIcon = @MenuIcon,
                         IsActive = @IsActive,
                         ProjectName = @ProjectName,
                         DisplayOrder = @DisplayOrder
                     WHERE MenuId = @MenuId";  // Ensure you update the correct record by MenuId

            // Handle the nullable ParentId parameter
            var parentIdParam = new SqlParameter("@ParentId", rptMenu.ParentId.HasValue && rptMenu.ParentId != 0 ? (object)rptMenu.ParentId : DBNull.Value);

            // Execute the SQL query with parameters
            _dbContext.Database.ExecuteSqlRaw(sqlQuery,
                             new SqlParameter("@ModuleId", rptMenu.ModuleId),
                             parentIdParam,
                             new SqlParameter("@DisplayName", rptMenu.DisplayName),
                             new SqlParameter("@ActionResult", rptMenu.ActionResult),
                             new SqlParameter("@Controller", rptMenu.Controller),
                             new SqlParameter("@MenuIcon", rptMenu.MenuIcon),
                             new SqlParameter("@IsActive", rptMenu.IsActive),
                             new SqlParameter("@ProjectName", rptMenu.ProjectName),
                             new SqlParameter("@DisplayOrder", rptMenu.DisplayOrder),
                             new SqlParameter("@MenuId", rptMenu.MenuId));  // Use MenuId to update the correct record

            // Return the updated MenuId
            return rptMenu.ModuleId;
        }

        public List<RptMenu> GetAllMenuListWithUserRoleAndCompany(string UserRole, int companyId)
        {
            try
            {
                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 9);
                p.Add("@UserRole", UserRole);
                p.Add("@companyId", companyId);
                List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int UpdateCompanyWiseMenu(CompanyWiseMenuUpdate menu)
        {
            string SP = "SP_GETMENU";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 12);
            p.Add("@UserRole", menu.UserRole);
            p.Add("@companyId", menu.CompId);
            p.Add("@MenuId", menu.MenuId);

            int ItemList = _IDapperService.GetAllBySP<CompanyWiseMenuUpdate>(SP, p).Count();

            if (ItemList > 0)
            {
                // Update existing record
                var sqlUpdateQuery = @"UPDATE tblMVCRoleAccess 
                               SET Access = @IsActive,
                                   ModifiedDate = @UpdateDate,
                                   ModifiedBy = @UpdateBy
                               WHERE MenuId = @MenuId 
                               AND CompId = @CompId 
                               AND Role = @UserRole";

                _dbContext.Database.ExecuteSqlRaw(sqlUpdateQuery,
                    new SqlParameter("@UpdateDate", DateTime.Now),
                    new SqlParameter("@UpdateBy", menu.UpdateBy),
                    new SqlParameter("@IsActive", menu.IsActive),
                    new SqlParameter("@MenuId", menu.MenuId),
                    new SqlParameter("@CompId", menu.CompId),
                    new SqlParameter("@UserRole", menu.UserRole));
            }
            else
            {
                // Insert new record
                var sqlInsertQuery = @"INSERT INTO tblMVCRoleAccess (MenuId, CompId,Role, Access, CreatedDate, CreatedBy)
                               VALUES (@MenuId, @CompId,@Role, @Access, @CreatedDate, @CreatedBy)";

                _dbContext.Database.ExecuteSqlRaw(sqlInsertQuery,
                    new SqlParameter("@MenuId", menu.MenuId),
                    new SqlParameter("@CompId", menu.CompId),
                    new SqlParameter("@Role", menu.UserRole),
                    new SqlParameter("@Access", menu.IsActive),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@CreatedBy", menu.EntryBy));
            }

            return menu.MenuId;
        }



        public List<RptMenu> GetAllMenuListWithUserRoleComapnyModule(string UserRole, int companyId, int moduleId)
        {
            try
            {

                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 10);
                p.Add("@UserRole", UserRole);
                p.Add("@companyId", companyId);
                p.Add("@moduleId", moduleId);

                List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).OrderBy(x => x.DisplayOrder).ToList();
                return ItemList;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
