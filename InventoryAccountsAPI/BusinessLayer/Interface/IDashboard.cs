
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IDashboard: ICommonMethod<Dashboard>
    {
        public List<Dashboard> GetTotalSalesAndProfitByWhId(int whId, DateTime FromDt, DateTime Todt);

        public List<TopSalesProduct> GetTopSalesByWhId(int whId, DateTime FromDt, DateTime Todt);
        public List<YearWiseProfit> YearWiseProfitByWhId(int whId, int year);
    }
}
