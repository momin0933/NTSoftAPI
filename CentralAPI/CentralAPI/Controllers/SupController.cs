using CentralAPI.BusinessLayer.Interface;
using CentralAPI.Models.ReportModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CentralAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupController : ControllerBase
    {
        private readonly ISupBill _ServiceManager;
        public SupController(ISupBill SubBIll)
        {
            _ServiceManager = SubBIll;
        }


        [HttpGet("GetSubBill")]
        [HttpGet]
        public ActionResult<List<RptSubBill>> GetSubBill([FromQuery] string projectName, [FromQuery] DateTime invoiceDt)
        {
            List<RptSubBill> itemList = _ServiceManager.GetSubBill(projectName, invoiceDt);
            return Ok(itemList);
        }
    }
}
