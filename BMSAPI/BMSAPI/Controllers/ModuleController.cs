using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Manager;
using BMSAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModule _ServiceManager;

        public ModuleController(IModule Module)
        {
            _ServiceManager = Module;
        }

        [Route("api/Module/GetAllModuleList")]
        [HttpGet]
        public List<RptModule> GetAllModuleList(int companyId)
        {
            try
            {
                List<RptModule> itemList = _ServiceManager.GetAllModuleList(companyId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/Module/GetModuleById")]
        [HttpGet]
        public List<RptModule> GetModuleById(int moduleId)
        {
            try
            {
                List<RptModule> itemList = _ServiceManager.GetModuleById(moduleId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }


        [Route("api/Module/UpdateIsActive")]
        [HttpPut]
        public IActionResult UpdateIsActive(bool isActive, int moduleId)
        {
            try
            {

                var existingModule = _ServiceManager.GetModuleById(moduleId);

                if (existingModule != null)
                {

                    _ServiceManager.UpdateModule(isActive, moduleId);

                    return Ok(new { message = "Status updated successfully." });
                }

                return NotFound(new { message = "Module not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Route("api/Module/AddModule")]
        [HttpPost]
        public int Post([FromBody] RptModule entity)
        {
            _ServiceManager.Add(entity);
            return entity.ModuleId;
        }

        [Route("api/Module/GetModuleByCompanyId")]
        [HttpGet]
        public List<ModuleDto> GetModuleByCompanyId(int companyId)
        {
            try
            {
                List<ModuleDto> itemList = _ServiceManager.GetCompanyWiseModule(companyId);
                return itemList;
            }
            catch
            {
                throw;
            }
        }
        [Route("api/Module/AddCompanyModules")]
        [HttpPost]
        public IActionResult UpdateCompanyModules(CompanyModuleDto dto)
        {
            var existingMappings = _ServiceManager.GetCompanyModuleMap(dto.CompanyId);

            foreach (var mapping in existingMappings)
            {

                if (!dto.ModuleIds.Contains(mapping.ModuleId))
                {
                    // If the module is currently active, mark it as inactive
                    if (mapping.IsActive == true)
                    {
                        mapping.IsActive = false;
                        _ServiceManager.UpdateCompanyModuleMap(mapping);
                    }
                }
                else
                {
                    // If the module is checked (exists in dto.ModuleIds)
                    // If it was inactive, we need to mark it as active
                    if (mapping.IsActive == false)
                    {
                        mapping.IsActive = true;
                        _ServiceManager.UpdateCompanyModuleMap(mapping);
                    }
                }
            }


            foreach (var moduleId in dto.ModuleIds)
            {
                var existingMapping = existingMappings.FirstOrDefault(m => m.ModuleId == moduleId);

                if (existingMapping == null)
                {
                    var customerModuleMap = new CompanyModuleMap
                    {
                        CompanyId = dto.CompanyId,
                        ModuleId = moduleId,
                        IsActive = true
                    };


                    _ServiceManager.AddCompanyModuleMap(customerModuleMap);
                }
            }

            return Ok("Modules updated successfully.");
        }


    }
}
