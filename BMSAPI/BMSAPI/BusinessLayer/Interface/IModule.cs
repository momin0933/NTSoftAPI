using BMSAPI.BusinessLayer.Manager;
using BMSAPI.Models.Common;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IModule : ICommonMethod<RptModule>
    {
        List<RptModule> GetAllModuleList(int companyId);
        List<RptModule> GetModuleById(int moduleId);
        List<ModuleDto> GetCompanyWiseModule(int companyId);
        void UpdateModule(bool isActive, int moduleId);
        List<CompanyModuleMap> AddCompanyModuleMap(CompanyModuleMap customerModuleMap);
        void UpdateCompanyModuleMap(CompanyModuleMap customerModuleMap);

        List<CompanyModuleMap> GetCompanyModuleMap(int companyId);



    }
}
