using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ItemReturnManager :IItemReturn
    {
        //readonly FTDbContext _dbContext;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;

        //public ItemReturnManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}

        public ItemReturnManager(NTSoftDbContextFactory dbContext, IDapperService dapperService, ILogger<ItemManager> logger)
        {

            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
        }
        public List<POSItemReturn> GetAll()
        {
            var list = _ICommonService.GetAll<POSItemReturn>().OrderByDescending(x=>x.EntryDate).ToList();
            return list;

        }
        public POSItemReturn GetById(int Id)
        {
            var itemReturn = _ICommonService.Get<POSItemReturn>(Id);
            return itemReturn;
        }
        public int Add(POSItemReturn entity)
        {
            _ICommonService.Add<POSItemReturn>(entity);
            return entity.Id;
        }
        public int Update(POSItemReturn entity)
        {
            _ICommonService.Update<POSItemReturn>(entity);  
            return entity.Id;
        }
        //public int Delete(int id) 
        //{
        //    _ICommonService.RemoveById<POSItemReturn>(id);
        //    return id;
        
        //}

        public int CountReturnInvoice(string compCode)
        {
            //string currentYearMonthDay = DateTime.Now.ToString("yyMMdd"); 
            string currentYearMonth = DateTime.Now.ToString("yyMM");

            // Get last invoice for this month
            var lastInvoice = _ICommonService.Query<Invoice>()
                .Where(i =>
                    !string.IsNullOrEmpty(i.InvoiceNo) &&
                    i.InvoiceNo.StartsWith("RET" + compCode)&&                 
                    i.InvoiceNo.Contains(currentYearMonth)
                )
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            if (lastInvoice != null)
            {
                // Extract counter: everything after SI-YYMMDD
                string counterStr = lastInvoice.InvoiceNo.Substring(14); // 3 + 6 = 9 → after RETYYMMDD
                if (long.TryParse(counterStr, out long lastCounter))
                    return (int)(lastCounter + 1); // next counter
            }

            // First invoice today
            return 1;
        }

        public List<RptItem> GetAllItemStockListWithPrice(int WHId, string type)
        {
            try
            {
                string SP = "SP_ItemStock";
                DynamicParameters p = new DynamicParameters();
                if (type == "Details")
                {
                    p.Add("@QueryChecker", 5);
                }
                //else
                //{
                //    p.Add("@QueryChecker", 2);

                //}
                p.Add("@WHId", WHId);

                List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x => x.EntryDate).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    
}
