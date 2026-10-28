using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class DashboardManager : IDashboard
    {

        //readonly FTDbContext _dbContext;
        readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;

        //public DashboardManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public DashboardManager(NTSoftDbContext dbContext, DapperService dapperService, NTSoftDbContextFactory dbContextFactory)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(dbContextFactory);
        }
        public int Add(Dashboard entity)
        {
            throw new NotImplementedException();
        }

        public List<Dashboard> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Dashboard> GetTotalSalesAndProfitByWhId(int whId, DateTime FromDt, DateTime Todt)
        {
            try
            {
                string SP = "SP_GetProfitAndSales";
                DynamicParameters p = new DynamicParameters();

                p.Add("@whid", whId);
                p.Add("@FromDt", FromDt);
                p.Add("@Todt", Todt);

                List<Dashboard> list = _IDapperService.GetAllBySP<Dashboard>(SP, p).ToList();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TopSalesProduct> GetTopSalesByWhId(int whId, DateTime FromDt, DateTime Todt)
        {
            try
            {
                string SP = "SP_GetTop5Sales";
                DynamicParameters p = new DynamicParameters();

                p.Add("@whid", whId);
                p.Add("@FromDt", FromDt);
                p.Add("@Todt", Todt);

                List<TopSalesProduct> list = _IDapperService.GetAllBySP<TopSalesProduct>(SP, p).ToList();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<YearWiseProfit> YearWiseProfitByWhId(int whId, int year)
        {
            try
            {
                string SP = "SP_GetYearWiseProfit";
                DynamicParameters p = new DynamicParameters();

                p.Add("@whid", whId);
                p.Add("@year", year);


                List<YearWiseProfit> list = _IDapperService.GetAllBySP<YearWiseProfit>(SP, p).ToList();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Remove(Dashboard entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Dashboard entity)
        {
            throw new NotImplementedException();
        }
    }
}
