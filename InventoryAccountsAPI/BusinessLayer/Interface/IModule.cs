
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IModule : ICommonMethod<RptModule>
    {
        List<RptModule> GetAllModuleList(int companyId);
        List<RptModule> GetModuleById(int moduleId);
        List<ModuleDto> GetCompanyWiseModule(int companyId);
        void UpdateModule( bool isActive,int moduleId);
        List<CompanyModuleMap> AddCompanyModuleMap(CompanyModuleMap customerModuleMap);
        void UpdateCompanyModuleMap(CompanyModuleMap customerModuleMap);

        List<CompanyModuleMap> GetCompanyModuleMap(int companyId);
        


    }
}
