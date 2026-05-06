using CentralAPI.BusinessLayer.Interface;
using CentralAPI.Models.ReportModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenu _ServiceManager;

        public MenuController(IMenu Menu)
        {
            _ServiceManager = Menu;
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
    }
}
