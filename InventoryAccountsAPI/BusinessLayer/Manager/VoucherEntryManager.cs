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
    public class VoucherEntryManager : IAcctblVoucher
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //public VoucherEntryManager(NTSoftDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public VoucherEntryManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }
        public int Add(VoucherDetails entity)
        {
            try
            {
                
                _ICommonService.Add<VoucherDetails>(entity);
            
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<VoucherDetails> GetById(int Id)
        {
            var VoucherEntry = await _dbContext.VoucherEntry.FindAsync(Id);
            return VoucherEntry;
        }


        public int Remove(VoucherDetails entity)
        {
            try
            {
                _ICommonService.RemoveById<VoucherDetails>(entity.Id);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }


        public int Update(VoucherDetails entity)
        {
            try
            {
                _ICommonService.Update<VoucherDetails>(entity);
                //var rcvQty = _ICommonService.GetAll<Item>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).Sum(c => (int?)c.Qty) ?? 0;
                //var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).FirstOrDefault();
                //if (stock != null)
                //{
                //    stock.ReceiveQty = rcvQty;
                //    stock.StockQty = stock.IssueQty == null ? rcvQty : rcvQty + stock.IssueQty;
                //}

                //_ICommonService.Update<ItemStock>(stock);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public int Update(AccGLedger entity)
        {
            throw new NotImplementedException();
        }
        
        public RptAccVoucher GetVocherById(int id)
        {
            try
            {
                RptAccVoucher voucher = new RptAccVoucher();
                List<RptVoucherDetails> details = new List<RptVoucherDetails>();
                string query = "SELECT Il.Title AS LedgerName, v.* FROM AcctblVoucherDetails AS v JOIN AcctblILedger AS Il ON v.LedgerId = Il.Id;";
                details = _IDapperService.GetAllByQuery<RptVoucherDetails>(query).ToList();
                if (voucher != null)
                {
                    details = details.Where(x=>x.AccVoucherId == id).ToList();
                    voucher.rptVoucherDetails = details;
                }
                return voucher;
            }
            catch
            {
                throw;
            }
        }

        Task<AccVoucher> IAcctblVoucher.GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<AccVoucher> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Add(AccVoucher entity)
        {
            throw new NotImplementedException();
        }

        public int Update(AccVoucher entity)
        {
            throw new NotImplementedException();
        }

        public int Remove(AccVoucher entity)
        {
            throw new NotImplementedException();
        }

        public List<RptAccVoucherListWithDetails> ACCGetAllVoucherListWithDetails(DateTime fromDate, DateTime toDate, string? vhoucherType, string? voucherNumber, int companyId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetVoucherNoByVoucherType(DateTime fromDate, DateTime toDate, string voucherType)
        {
            throw new NotImplementedException();
        }

        public int UpdateOrAdd(AccVoucher entity)
        {
            throw new NotImplementedException();
        }
    }
}
