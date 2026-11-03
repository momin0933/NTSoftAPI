using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ModuleManager : IModule
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //public ModuleManager(NTSoftDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public ModuleManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }
        public int Add(RptModule entity)
        {
            var sqlQuery = @"INSERT INTO FashionTexLive.dbo.tblMVCModule 
                           (ModuleName,ModuleIcon, ModuleLblName, ModuleLblIcon, isActive, ModuleSorting)
                           VALUES (@ModuleName,@ModuleIcon,@ModuleLblName,@ModuleLblIcon,@isActive,@ModuleSorting);"
            ;
            _dbContext.Database.ExecuteSqlRaw(sqlQuery,
                            new SqlParameter("@ModuleName", entity.ModuleName),
                            new SqlParameter("@ModuleIcon", entity.ModuleIcon),
                            new SqlParameter("@ModuleLblName", entity.ModuleLblName ?? (object)DBNull.Value),
                            new SqlParameter("@ModuleLblIcon", entity.ModuleLblIcon ?? (object)DBNull.Value),
                            new SqlParameter("@isActive", entity.IsActive),
                            new SqlParameter("@ModuleSorting", entity.ModuleSorting));
            return entity.ModuleId;
        }

      
        public List<RptModule> GetAllModuleList(int companyId)
        {
            if(companyId == 0)
            {
                string SP = "SP_GetModule";
                DynamicParameters p = new DynamicParameters();
                p.Add("QueryChecker", 2);
                p.Add("@companyId", companyId);
                p.Add("@ModuleId", "");
                List<RptModule> ItemList = _IDapperService.GetAllBySP<RptModule>(SP, p).OrderBy(x => x.ModuleSorting).ToList();
                return ItemList;
            }
            else
            {
                string SP = "SP_GetModule";
                DynamicParameters p = new DynamicParameters();
                p.Add("QueryChecker", 3);
                p.Add("@companyId", companyId);
                p.Add("@ModuleId", "");
                List<RptModule> ItemList = _IDapperService.GetAllBySP<RptModule>(SP, p).ToList();
                return ItemList;
            }
        }

        public List<RptModule> GetModuleById(int moduleId)
        {
            try
            {
                string SP = "SP_GetModule";
                DynamicParameters p = new DynamicParameters();
                p.Add("QueryChecker", 1);
                p.Add("@ModuleId", moduleId);
                p.Add("@companyId", "");

                List<RptModule> ItemList = _IDapperService.GetAllBySP<RptModule>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UpdateModule(bool isActive, int moduleId)
        {
            string query = $"UPDATE tblMvcModule SET IsActive = {(isActive ? 1 : 0)} WHERE ModuleId = {moduleId}";
            int list = _IDapperService.UpdateByQuery(query);
        }

        List<RptModule> ICommonMethod<RptModule>.GetAll()
        {
            throw new NotImplementedException();
        }

        public int Update(RptModule entity)
        {
            throw new NotImplementedException();
        }

        public int Remove(RptModule entity)
        {
            throw new NotImplementedException();
        }

        public List<ModuleDto> GetCompanyWiseModule(int companyId)
        {
            try
            {
                string SP = "SP_GetModule";
                DynamicParameters p = new DynamicParameters();
                p.Add("QueryChecker", 4);

                p.Add("@companyId", companyId);
                p.Add("@moduleId", "");

                List<ModuleDto> ItemList = _IDapperService.GetAllBySP<ModuleDto>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<CompanyModuleMap> AddCompanyModuleMap(CompanyModuleMap customerModuleMap)
        {
            _ICommonService.Add<CompanyModuleMap>(customerModuleMap);
            return _ICommonService.GetAll<CompanyModuleMap>().ToList();
        }
        public void UpdateCompanyModuleMap(CompanyModuleMap customerModuleMap)
        {
            _ICommonService.Update<CompanyModuleMap>(customerModuleMap);
            _dbContext.SaveChanges();
        }
        public List<CompanyModuleMap> GetCompanyModuleMap(int companyId)
        {
            return _dbContext.tblCompanyModuleMap.Where(c => c.CompanyId == companyId).ToList();
        }

        
    }
}