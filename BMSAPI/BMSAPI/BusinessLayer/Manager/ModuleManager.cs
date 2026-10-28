using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using BMSAPI.Models.Common;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class ModuleManager : IModule
    {
        private const string SP = "SP_GetModule";

        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _dapper;

        public ModuleManager(NTSoftDbContextFactory dbFactory, IDapperService dapper)
        {
            _dbFactory = dbFactory;
            _dapper = dapper;
        }

        // ---------------- Module ----------------

        public int Add(RptModule entity)
        {
            // নতুন module-এর আসল Id ফেরত দেওয়া হয়
            return _dapper.GetSingle<int>(@"
                INSERT INTO dbo.tblMVCModule
                    (ModuleName, ModuleIcon, ModuleLblName, ModuleLblIcon, IsActive, ModuleSorting)
                VALUES
                    (@ModuleName, @ModuleIcon, @ModuleLblName, @ModuleLblIcon, @IsActive, @ModuleSorting);
                SELECT CAST(SCOPE_IDENTITY() AS int);",
                new
                {
                    entity.ModuleName,
                    entity.ModuleIcon,
                    entity.ModuleLblName,
                    entity.ModuleLblIcon,
                    entity.IsActive,
                    entity.ModuleSorting
                });
        }

        public List<RptModule> GetAllModuleList(int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", companyId == 0 ? 2 : 3);
            p.Add("@companyId", companyId);
            p.Add("@ModuleId", "");

            var list = _dapper.GetAllBySP<RptModule>(SP, p);

            return companyId == 0
                ? list.OrderBy(x => x.ModuleSorting).ToList()
                : list.ToList();
        }

        public List<RptModule> GetModuleById(int moduleId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@ModuleId", moduleId);
            p.Add("@companyId", "");
            return _dapper.GetAllBySP<RptModule>(SP, p).ToList();
        }

        public void UpdateModule(bool isActive, int moduleId)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Database.GetDbConnection().Execute(
                "UPDATE dbo.tblMVCModule SET IsActive = @isActive WHERE ModuleId = @moduleId",
                new { isActive, moduleId });
        }

        public List<ModuleDto> GetCompanyWiseModule(int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 4);
            p.Add("@companyId", companyId);
            p.Add("@moduleId", "");
            return _dapper.GetAllBySP<ModuleDto>(SP, p).ToList();
        }

        // ---------------- Company ↔ Module map ----------------

        public List<CompanyModuleMap> AddCompanyModuleMap(CompanyModuleMap customerModuleMap)
        {
            using var db = _dbFactory.CreateDbContext();
            db.CompanyModuleMaps.Add(customerModuleMap);
            db.SaveChanges();

            return db.CompanyModuleMaps.AsNoTracking().ToList();
        }

        public void UpdateCompanyModuleMap(CompanyModuleMap customerModuleMap)
        {
            using var db = _dbFactory.CreateDbContext();
            db.CompanyModuleMaps.Update(customerModuleMap);
            db.SaveChanges();
        }

        public List<CompanyModuleMap> GetCompanyModuleMap(int companyId)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.CompanyModuleMaps.AsNoTracking()
                .Where(c => c.CompanyId == companyId)
                .ToList();
        }

        // ---------------- ICommonMethod (ব্যবহার হয় না) ----------------

        public List<RptModule> GetAll() => throw new NotImplementedException();

        public RptModule? Get(int id) => GetModuleById(id).FirstOrDefault();

        public int Update(RptModule entity) => throw new NotImplementedException();

        public int Remove(RptModule entity) => throw new NotImplementedException();
    }
}