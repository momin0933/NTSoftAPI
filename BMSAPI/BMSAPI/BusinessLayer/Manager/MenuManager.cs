using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Common;
using BMSAPI.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class MenuManager : IMenu
    {
        private const string SP = "SP_GETMENU";

        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _dapper;

        public MenuManager(NTSoftDbContextFactory dbFactory, IDapperService dapper)
        {
            _dbFactory = dbFactory;
            _dapper = dapper;
        }

        // ---------------- Menu queries (SP_GETMENU) ----------------

        public List<RptMenu> GetAllMenuListWithUserRole(string UserRole, string projectName)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@UserRole", UserRole);
            p.Add("@projectName", projectName);
            return _dapper.GetAllBySP<RptMenu>(SP, p).OrderBy(x => x.ModuleSorting).ToList();
        }

        public List<RptMenu> GetAllMenuList(int ModuleId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@ModuleId", ModuleId);
            return _dapper.GetAllBySP<RptMenu>(SP, p).ToList();
        }

        public List<Project> GetAllProjectList()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 3);
            return _dapper.GetAllBySP<Project>(SP, p).ToList();
        }

        public List<UserRoles> GetAllRoleList(int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", companyId != 0 ? 4 : 11);
            p.Add("@companyId", companyId);
            return _dapper.GetAllBySP<UserRoles>(SP, p).ToList();
        }

        public List<RptMenu> GetMenuList(int moduleId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", moduleId == 0 ? 7 : 8);
            p.Add("@moduleId", moduleId);
            return _dapper.GetAllBySP<RptMenu>(SP, p).ToList();
        }

        public List<RptMenu> GetMenuById(int menuId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 6);
            p.Add("@MenuId", menuId);
            return _dapper.GetAllBySP<RptMenu>(SP, p).ToList();
        }

        public List<RptMenu> GetAllMenuListWithUserRoleAndCompany(string UserRole, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 9);
            p.Add("@UserRole", UserRole);
            p.Add("@companyId", companyId);
            return _dapper.GetAllBySP<RptMenu>(SP, p).ToList();
        }

        public List<RptMenu> GetAllMenuListWithUserRoleComapnyModule(string UserRole, int companyId, int moduleId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 10);
            p.Add("@UserRole", UserRole);
            p.Add("@companyId", companyId);
            p.Add("@moduleId", moduleId);
            return _dapper.GetAllBySP<RptMenu>(SP, p).OrderBy(x => x.DisplayOrder).ToList();
        }

        // ---------------- Role access ----------------

        public bool PostRoleAccess(int CompanyId, List<RoleAccess> itemList)
        {
            if (itemList == null || itemList.Count == 0)
                return false;

            // ১) এই role-এর পুরনো access মুছে ফেলা
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 0);
            p.Add("@CompanyId", CompanyId);
            p.Add("@UserRole", itemList[0].Role);
            _dapper.PostBySP(SP, p);

            // ২) নতুন access বসানো
            var saved = _dapper.PostBulkInsert(itemList, "tblMVCRoleAccess");
            if (!saved)
                throw new Exception($"Old access for role '{itemList[0].Role}' was removed but saving the new access list failed. Please save again.");

            return true;
        }

        public int UpdateCompanyWiseMenu(CompanyWiseMenuUpdate menu)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 12);
            p.Add("@UserRole", menu.UserRole);
            p.Add("@companyId", menu.CompId);
            p.Add("@MenuId", menu.MenuId);

            bool exists = _dapper.GetAllBySP<CompanyWiseMenuUpdate>(SP, p).Any();

            using var db = _dbFactory.CreateDbContext();
            var conn = db.Database.GetDbConnection();

            if (exists)
            {
                conn.Execute(@"UPDATE tblMVCRoleAccess
                               SET Access = @IsActive, ModifiedDate = @UpdateDate, ModifiedBy = @UpdateBy
                               WHERE MenuId = @MenuId AND CompId = @CompId AND Role = @UserRole",
                    new
                    {
                        menu.IsActive,
                        UpdateDate = DateTime.Now,
                        menu.UpdateBy,
                        menu.MenuId,
                        menu.CompId,
                        menu.UserRole
                    });
            }
            else
            {
                conn.Execute(@"INSERT INTO tblMVCRoleAccess (MenuId, CompId, Role, Access, CreatedDate, CreatedBy)
                               VALUES (@MenuId, @CompId, @Role, @Access, @CreatedDate, @CreatedBy)",
                    new
                    {
                        menu.MenuId,
                        menu.CompId,
                        Role = menu.UserRole,
                        Access = menu.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = menu.EntryBy
                    });
            }

            return menu.MenuId;
        }

        // ---------------- Menu add / update ----------------

        public int Add(MenuViewModel menuVM)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Database.GetDbConnection().Execute(@"
                INSERT INTO dbo.tblMVCMenu
                    (ModuleId, ParentId, DisplayName, ActionResult, Controller, MenuIcon, IsActive, ProjectName, DisplayOrder)
                VALUES
                    (@ModuleId, @ParentId, @DisplayName, @ActionResult, @Controller, @MenuIcon, @IsActive, @ProjectName, @DisplayOrder);",
                new
                {
                    menuVM.ModuleId,
                    ParentId = menuVM.ParentId > 0 ? menuVM.ParentId : null,
                    menuVM.DisplayName,
                    menuVM.ActionResult,
                    menuVM.Controller,
                    menuVM.MenuIcon,
                    menuVM.IsActive,
                    menuVM.ProjectName,
                    menuVM.DisplayOrder
                });

            return menuVM.ModuleId;
        }

        public int Update(MenuViewModel rptMenu)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Database.GetDbConnection().Execute(@"
                UPDATE dbo.tblMVCMenu
                SET ModuleId = @ModuleId,
                    ParentId = @ParentId,
                    DisplayName = @DisplayName,
                    ActionResult = @ActionResult,
                    Controller = @Controller,
                    MenuIcon = @MenuIcon,
                    IsActive = @IsActive,
                    ProjectName = @ProjectName,
                    DisplayOrder = @DisplayOrder
                WHERE MenuId = @MenuId;",
                new
                {
                    rptMenu.ModuleId,
                    ParentId = rptMenu.ParentId > 0 ? rptMenu.ParentId : null,
                    rptMenu.DisplayName,
                    rptMenu.ActionResult,
                    rptMenu.Controller,
                    rptMenu.MenuIcon,
                    rptMenu.IsActive,
                    rptMenu.ProjectName,
                    rptMenu.DisplayOrder,
                    rptMenu.MenuId
                });

            return rptMenu.ModuleId;
        }

        // ---------------- UserRoles CRUD (ICommonMethod) ----------------

        public List<UserRoles> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.UserRoles.AsNoTracking().ToList();
        }

        public int Add(UserRoles entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.UserRoles.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(UserRoles entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.UserRoles.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Remove(UserRoles entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.UserRoles.Remove(entity);
            db.SaveChanges();
            return entity.Id;
        }
    }
}
