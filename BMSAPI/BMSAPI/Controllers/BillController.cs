using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBill _Service;

        public BillController(IBill bill)
        {
            _Service = bill;
        }

        [Route("GetBillByFlatCode")]
        [HttpGet]
        public List<RptBill> GetBillByFlatCode(string FlatCode)
        {
            return _Service.GetBillByFlatCode(FlatCode);
        }

        [Route("UpdateBill")]
        [HttpPost]
        public int Post([FromBody] ViewModalBill entity)
        {
            return _Service.UpdateBill(entity);
        }

        [Route("AddBill")]
        [HttpPost]
        public int AddBill()
        {
            return _Service.AddBill();
        }
    }
}