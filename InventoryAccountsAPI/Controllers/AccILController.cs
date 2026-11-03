
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccILController : ControllerBase
    {

        private readonly IAccLevelTwo _AccLevelTwo;


        public AccILController(IAccLevelTwo AccLevelTwo)
        {
            _AccLevelTwo = AccLevelTwo;
            
        }


        [Route("GetAll")]
        [HttpGet]
        public List<AccILedger> GetAll()
        {
            try
            {
                List<AccILedger> AccLevelTwoList = new List<AccILedger>();

                AccLevelTwoList = _AccLevelTwo.GetAll();
                return AccLevelTwoList;
            }
            catch
            {
                throw;
            }
        }

        [Route("Add")]
        [HttpPost]
        public int Post([FromBody] AccILedger entity)
        {
            _AccLevelTwo.Add(entity);
            return entity.Id;
        }


        [Route("Update")]
        [HttpPost]
        public int Put([FromBody] AccILedger entity)
        {
            _AccLevelTwo.Update(entity);
            return entity.Id;
        }




        [Route("GetById")]
        [HttpGet]
        public async Task<IActionResult> GetbyId(int Id)
        {
            try
            {
                var AccLevelTwo = await _AccLevelTwo.GetById(Id);

                if (AccLevelTwo != null)
                {
                    // Return a 200 OK response with the retrieved account
                    return Ok(AccLevelTwo);
                }
                else
                {
                    // Return a 404 Not Found response if the account is not found
                    return NotFound($"ID {Id} not found");
                }
            }

            catch (Exception ex)
            {
                // Handle other exceptions and return a 500 Internal Server Error response
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("GetAllGledgerWithIlegder")]
        [HttpGet]
        public List<RptILedger> GetAllGledgerWithIlegder()
        {
            var list = _AccLevelTwo.GetAllGledgerWithIlegder();
            return list;
        }

        [Route("GetAllLedgerForDropdown")]
        [HttpGet]
        public List<BaseDropDown> GetAllLedgerForDropdown(DateTime fromDate, DateTime toDate)
        {
            var list = _AccLevelTwo.GetAllLedgerForDropdown(fromDate, toDate);
            return list;    
        }
        [Route("GetLedgerListWithVoucherDetails")]
        [HttpGet]
        public List<RptLedgerReport>GetLedgerListWithVoucherDetails(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId)
        {
            var list = _AccLevelTwo.GetLedgerListWithVoucherDetails(fromDate, toDate, ledgerId,companyId);
            return list;
        }

        [Route("GetLedgerListWithBalance")]
        [HttpGet]
        public List<RptLedgerReport> GetLedgerListWithBalance(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId, int? supplierId)
        {
            var list = _AccLevelTwo.GetLedgerListWithBalance(fromDate, toDate, ledgerId,companyId, supplierId);
            return list;
        }


        [Route("GetLedgerWithTrialBalance")]
        [HttpGet]
        public List<RptLedger> GetLedgerWithTrialBalance(DateTime fromDate, DateTime toDate,int companyId)
        {
            var list = _AccLevelTwo.GetLedgerWithTrialBalance(fromDate, toDate, companyId);
            return list;
        }



        [Route("IncomeStatement")]
        [HttpGet]
        public List<RptProfitLoss> IncomeStatement(DateTime fromDate, DateTime toDate, int companyId)
        {
            var list = _AccLevelTwo.IncomeStatement(fromDate, toDate,companyId);
            return list;
        }
        [Route("BalanceSheet")]
        [HttpGet]
        public List<RptProfitLoss> BalanceSheet(DateTime fromDate, DateTime toDate,int companyId)
        {
            var list = _AccLevelTwo.BalanceSheet(fromDate, toDate, companyId);
            return list;
        }     

        [Route("ReceiptNPayment")]
        [HttpGet]
        public List<RptReceiptPayment> ReceiptNPayment(DateTime fromDate, DateTime toDate,int companyId)
        {
            var list = _AccLevelTwo.ReceiptNPayment(fromDate, toDate, companyId);
            return list;
        }
    }
}
