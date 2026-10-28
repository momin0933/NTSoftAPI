using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.BusinessLayer.Interface;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDashboard _idashboard;

        public HomeController(IDashboard idashboard)
        {
            _idashboard = idashboard;
        }


        [Route("GetTotalSalesAndProfit")]
        [HttpGet]
        public List<Dashboard> GetTotalSalesAndProfitByWhId(int whId,DateTime FromDt,DateTime ToDt)
        {
            var CurrentMonthProfitSales = _idashboard.GetTotalSalesAndProfitByWhId(whId,FromDt,ToDt);
            return CurrentMonthProfitSales;
        }

        [Route("GetTopSalesProduct")]
        [HttpGet]
        public List<TopSalesProduct> GetTopSalesByWhId(int whId, DateTime FromDt, DateTime ToDt)
        {
            var CurrentMonthProfitSales = _idashboard.GetTopSalesByWhId(whId, FromDt, ToDt);
            return CurrentMonthProfitSales;
        }

        [Route("GetYearWiseProfit")]
        [HttpGet]
        public List<YearWiseProfit> YearWiseProfitByWhId(int whId, int year)
        {
            var CurrentMonthProfitSales = _idashboard.YearWiseProfitByWhId(whId, year);
            return CurrentMonthProfitSales;
        }
    }
}
