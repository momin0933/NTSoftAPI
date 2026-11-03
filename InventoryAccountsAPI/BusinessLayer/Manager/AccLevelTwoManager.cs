using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSoftAPI.Service.BusinessLayer.Manager
{
    public class AccLevelTwoManager : IAccLevelTwo
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        private readonly string _connectionString;
        SQLConnectionString _connectionStringService = new SQLConnectionString();
        //public AccLevelTwoManager(NTSoftDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //    _connectionString = _connectionStringService.GetConnectionString("default");
        //}

        public AccLevelTwoManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
            _connectionString = _connectionStringService.GetConnectionString("default");
        }
        public int Add(AccILedger entity)
        {
            try
            {
                
                _ICommonService.Add<AccILedger>(entity);

            
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

 

        public List<AccILedger> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<AccILedger>().Where(x=> x.IsActive == true).ToList();
                
            }
            catch
            {
                throw;
            }           
            
        }


 



        public async Task<AccILedger> GetById(int Id)
        {
            var AccLevelTwo = await _dbContext.AccLevelTwo.FindAsync(Id);
            return AccLevelTwo;
        }

  
        public int Remove(AccILedger entity)
        {
            try
            {
                _ICommonService.RemoveById<AccILedger>(entity.Id);
        
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }


        public int Update(AccILedger entity)
        {
            try
            {
                _ICommonService.Update<AccILedger>(entity);            
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        //public int Update(Accounts entity)
        //{
        //    throw new NotImplementedException();
        //}
        public List<RptILedger> GetAllGledgerWithIlegder()
        {
            
            try
            {
                string SP = "SP_AccCOA";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                //p.Add("@ProjectName", projectName);
                List<RptILedger> ItemList = _IDapperService.GetAllBySP<RptILedger>(SP, p).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<BaseDropDown> GetAllLedgerForDropdown(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string sp = "SP_ACC_GetLedgerReportDetails";
                DynamicParameters p = new DynamicParameters();
                p.Add("@fromDate", fromDate);
                p.Add("@toDate", toDate);
                p.Add("@querySelect", 1);
                p.Add("@ledgerId", null);
                List<BaseDropDown> dropDownList = _IDapperService.GetAllBySP<BaseDropDown>(sp, p).ToList();
                return dropDownList;
            }
            catch
            {
                throw;
            }

        }

        public List<RptLedgerReport> GetLedgerListWithVoucherDetails(DateTime fromDate, DateTime toDate,int? ledgerId, int? companyId)
        {
            try
            {
                string sp = "SP_ACC_GetLedgerReportDetails";
                DynamicParameters p = new DynamicParameters();
                p.Add("@fromDate", fromDate);
                p.Add("@toDate", toDate);
                p.Add("@ledgerId", ledgerId);
                p.Add("@companyId", companyId);
                if (ledgerId != 0 && companyId >0)
                {
                    p.Add("@querySelect", 2);
                }
                else if(ledgerId != 0 && companyId == 0)
                {
                    p.Add("@querySelect", 4);
                }
                else
                {
                    p.Add("@querySelect", 3);
                }
               
                
                List<RptLedgerReport> list = _IDapperService.GetAllBySP<RptLedgerReport>(sp,p).ToList();
                return list;
            }
            catch
            {
                throw;
            }

        }
        public List<RptLedgerReport> GetLedgerListWithBalance(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId, int? supplierId)
        {
            try
            {
                string sp = "SP_ACC_GetLedgerWithBalance";
                DynamicParameters p = new DynamicParameters();
                p.Add("@fromDate", fromDate);
                p.Add("@toDate", toDate);
                p.Add("@ledgerId", ledgerId);
                p.Add("@companyId", companyId);
                p.Add("@supplierId", supplierId);
                if (ledgerId != 0 && companyId > 0)
                {
                    p.Add("@querySelect", 2);
                }
                else if (ledgerId != 0 && companyId == 0)
                {
                    p.Add("@querySelect", 4);
                }
                else
                {
                    p.Add("@querySelect", 3);
                }


                List<RptLedgerReport> list = _IDapperService.GetAllBySP<RptLedgerReport>(sp, p).ToList();
                return list;
            }
            catch
            {
                throw;
            }

        }
        public List<RptLedger> GetLedgerWithTrialBalance(DateTime fromDate, DateTime toDate, int companyId)
        {
            try
            {

                if(companyId > 0)
                {
                    string sp = "SP_AccTrialBalance";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@companyId", companyId);
                    p.Add("@QueryChecker", 1);
                    List<RptLedger> list = _IDapperService.GetAllBySP<RptLedger>(sp, p).ToList();
                    return list;
                }
                else
                {
                    string sp = "SP_AccTrialBalance";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@companyId", companyId);
                    p.Add("@QueryChecker", 2);
                    List<RptLedger> list = _IDapperService.GetAllBySP<RptLedger>(sp, p).ToList();
                    return list;
                }
                
            }
            catch
            {
                throw;
            }



        }


        public List<RptProfitLoss> BalanceSheet(DateTime fromDate, DateTime toDate, int companyId)
        {
            try
            {
                if(companyId > 0)
                {
                    string sp = "Sp_AccIncomeStatement";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@QueryChecker", 2);
                    p.Add("@companyId", companyId);               
                    List<RptProfitLoss> list = _IDapperService.GetAllBySP<RptProfitLoss>(sp, p).ToList();
                    return list;
                }
                else
                {
                    string sp = "Sp_AccIncomeStatement";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@QueryChecker", 3);
                    p.Add("@companyId", companyId);               
                    List<RptProfitLoss> list = _IDapperService.GetAllBySP<RptProfitLoss>(sp, p).ToList();
                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<RptReceiptPayment> ReceiptNPayment(DateTime fromDate, DateTime toDate, int companyId)
        {
            if(companyId > 0)
            {
                string sp = "Sp_AccReceipt_Payment";
                DynamicParameters p = new DynamicParameters();
                p.Add("@fromDate", fromDate);
                p.Add("@toDate", toDate);
                p.Add("@companyid", companyId);

                p.Add("@QueryChecker", 1);
                //p.Add("@companyId", companyId);
                List<RptReceiptPayment> list = _IDapperService.GetAllBySP<RptReceiptPayment>(sp, p).ToList();
                return list;
            }
            else
            {
                string sp = "Sp_AccReceipt_Payment";
                DynamicParameters p = new DynamicParameters();
                p.Add("@fromDate", fromDate);
                p.Add("@toDate", toDate);
                p.Add("@companyid", companyId);

                p.Add("@QueryChecker", 2);
                //p.Add("@companyId", companyId);
                List<RptReceiptPayment> list = _IDapperService.GetAllBySP<RptReceiptPayment>(sp, p).ToList();
                return list;
            }
        }

        public List<RptProfitLoss> IncomeStatement(DateTime fromDate, DateTime toDate, int companyId)
        {
            try
            {

                if (companyId > 0)
                {
                    string sp = "Sp_AccIncomeStatement";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@companyId", companyId);
                    p.Add("@QueryChecker", 5);
                    List<RptProfitLoss> list = _IDapperService.GetAllBySP<RptProfitLoss>(sp, p).ToList();
                    return list;
                }
                else
                {
                    string sp = "Sp_AccIncomeStatement";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@QueryChecker", 4);
                    p.Add("@companyId", companyId);
                    List<RptProfitLoss> list = _IDapperService.GetAllBySP<RptProfitLoss>(sp, p).ToList();
                    return list;
                }

            }
            catch
            {
                throw;
            }
        }
    }
}
