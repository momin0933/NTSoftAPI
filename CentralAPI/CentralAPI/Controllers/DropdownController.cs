using CentralAPI.BusinessLayer.Interface;
using CentralAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdown _ServiceManager;

        public DropdownController(IDropdown dropdown)
        {
            _ServiceManager = dropdown;
        }

        [Route("api/Dropdown/GetAllBuyers")]
        [HttpGet]
        public List<BuyerDropdown> GetAllBuyers()
        {
            try
            {
                return _ServiceManager.GetAllBuyers();
            }
            catch { throw; }
        }

        [Route("api/Dropdown/GetAllCategories")]
        [HttpGet]
        public List<CategoryDropdown> GetAllCategories()
        {
            try
            {
                return _ServiceManager.GetAllCategories();
            }
            catch { throw; }
        }

        [Route("api/Dropdown/GetAllCustomers")]
        [HttpGet]
        public List<CustomerDropdown> GetAllCustomers()
        {
            try
            {
                return _ServiceManager.GetAllCustomers();
            }
            catch { throw; }
        }

        [Route("api/Dropdown/GetAllDepartments")]
        [HttpGet]
        public List<DepartmentDropdown> GetAllDepartments()
        {
            try
            {
                return _ServiceManager.GetAllDepartments();
            }
            catch { throw; }
        }

        [Route("api/Dropdown/GetAllFactories")]
        [HttpGet]
        public List<FactoryDropdown> GetAllFactories()
        {
            try
            {
                return _ServiceManager.GetAllFactories();
            }
            catch { throw; }
        }
        [Route("api/Dropdown/GetAllOrderSeasons")]
        [HttpGet]
        public List<OrderSeasonDropdown> GetAllOrderSeasons()
        {
            try
            {
                return _ServiceManager.GetAllOrderSeasons();
            }
            catch { throw; }
        }
        [Route("api/Dropdown/GetAllOrderTypes")]
        [HttpGet]
        public List<OrderTypeDropdown> GetAllOrderTypes()
        {
            try
            {
                return _ServiceManager.GetAllOrderTypes();
            }
            catch { throw; }
        }
    }
}