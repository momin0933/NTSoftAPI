
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccVoucherController : ControllerBase
    {


        private readonly IAcctblVoucher _AcctblVoucher;

        public AccVoucherController(IAcctblVoucher AcctblVoucher)
        {
            _AcctblVoucher = AcctblVoucher;
        }


        [Route("GetAll")]
        [HttpGet]
        public List<AccVoucher> GetAll()
        {
            try
            {
                List<AccVoucher> AcctblVoucherList = new List<AccVoucher>();

                AcctblVoucherList = _AcctblVoucher.GetAll();
                return AcctblVoucherList;
            }
            catch
            {
                throw;
            }
        }

        [Route("Add")]
        [HttpPost]
        public int Post([FromBody] AccVoucher entity)
        {
            _AcctblVoucher.Add(entity);
            return entity.Id;
        }
        [Route("GetById")]
        [HttpGet]
        public AccVoucher GetById(int id)
        {
            AccVoucher accVoucher = _AcctblVoucher.GetById(id).Result;
            return accVoucher;
        }

        [Route("GetVoucherWithById")]
        [HttpGet]
        public RptAccVoucher GetVoucherById(int id)
        {
            var dt = _AcctblVoucher.GetVocherById(id);
            return dt;

        }
        [Route("GetVoucherListWithDetails")]
        [HttpGet]
        public List<RptAccVoucherListWithDetails> GetVoucherListWithDetails(DateTime fromDate, DateTime toDate, string? vhoucherType, string? voucherNumber, int companyId)
        {
            if (voucherNumber != null)
            {
                voucherNumber = voucherNumber.Replace("-", "#");
            }
            var list = _AcctblVoucher.ACCGetAllVoucherListWithDetails(fromDate, toDate, vhoucherType, voucherNumber, companyId);
            return list;
        }
        [Route("GetVoucherNoByVoucherType")]
        [HttpGet]
        public List<string> GetVoucherNoByVoucherType(DateTime fromDate, DateTime toDate, string voucherType)
        {
            var list = _AcctblVoucher.GetVoucherNoByVoucherType(fromDate, toDate, voucherType);
            return list;
        }

        [Route("Update")]
        [HttpPost]
        public int Update([FromBody] AccVoucher entity)
        {
            _AcctblVoucher.UpdateOrAdd(entity);
            return entity.Id;
        }

        [Route("Remove")]
        [HttpPost]
        public int RemoveById([FromBody] AccVoucher entity)
        {
            _AcctblVoucher.Remove(entity);
            return entity.Id;
        }
    }
}
