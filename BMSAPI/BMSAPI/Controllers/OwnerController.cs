using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwner _Service;

        public OwnerController(IOwner owner)
        {
            _Service = owner;
        }

        [Route("Add")]
        [HttpPost]
        public int Add([FromBody] Owner entity)
        {
            return _Service.Add(entity);
        }

        [Route("GetBuildingWiseFlat")]
        [HttpGet]
        public List<Owner> GetOwnerWiseFlat()
        {
            return _Service.GetAll();
        }

        [Route("GetFlatWiseServiceCharge")]
        [HttpGet]
        public List<VwServiceCharge> GetFlatWiseServiceCharge([FromQuery] string year, [FromQuery] string month)
        {
            return _Service.GetFlatWiseServiceCharge(year, month);
        }

        [Route("GetFlatWiseOwnerList")]
        [HttpGet]
        public List<VwServiceCharge> GetFlatWiseOwnerList()
        {
            return _Service.GetFlatWiseOwnerList();
        }

        [Route("GetFlatWiseServiceChargeDetails")]
        [HttpGet]
        public VwServiceCharge GetFlatWiseServiceChargeDetails([FromQuery] string year, [FromQuery] string month, [FromQuery] int FCode)
        {
            return _Service.GetFlatWiseServiceChargeDetails(month, year, FCode);
        }

        [Route("UpdateOwner")]
        [HttpPost]
        public int UpdateOwner([FromBody] VwServiceCharge entity)
        {
            return _Service.UpdateOwner(entity);
        }

        [Route("UpdateTenant")]
        [HttpPost]
        public int UpdateTenant([FromBody] VwServiceCharge entity)
        {
            return _Service.UpdateTenant(entity);
        }
    }
}