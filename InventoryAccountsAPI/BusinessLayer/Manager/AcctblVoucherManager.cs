using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class AcctblVoucherManager : IAcctblVoucher
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        public AcctblVoucherManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }
        public int Add(AccVoucher entity)
        {
            try
            {
                
                _ICommonService.Add<AccVoucher>(entity);

                //// Now entity.Id is generated (assuming it's auto-incremented by DB)
                //entity.VoucherNumber = $"{entity.VoucherType}#{entity.Id}";

                //// Update the entity with the new VoucherNumber
                //_ICommonService.Update<AccVoucher>(entity);


                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

 

        public List<AccVoucher> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<AccVoucher>().ToList();
                
            }
            catch
            {
                throw;
            }
            
            
        }

        public async Task<AccVoucher> GetById(int Id)
        {
            var AcctblVoucher = await _dbContext.voucherEntryDetails.FindAsync(Id);
            return AcctblVoucher;
        }

        //public AccVoucher GetVocherById(int id)
        //{
        //    try
        //    {
        //        AccVoucher voucher = new AccVoucher();
        //        List<VoucherDetails> details = new List<VoucherDetails>();
        //        voucher = _dbContext.voucherEntryDetails.Where(x => x.Id == id).FirstOrDefault();
        //        if (voucher != null)
        //        {
        //            details = _dbContext.VoucherEntry.Where(list => list.AccVoucherId == voucher.Id).ToList();
        //            voucher.voucherEntryDetails = details;
        //        }
        //        return voucher;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public int Remove(AccVoucher entity)
        {
            try
            {

                var existingVoucher = _ICommonService.GetWithNoTracking<AccVoucher>(entity.Id);

                var existingDetail = _ICommonService
                 .GetByCondition<VoucherDetails>(d => d.AccVoucherId == entity.Id)
                 .AsNoTracking()
                 .ToList();

                var voucherLog = new AccVoucherLog
                {

                    VoucherId = existingVoucher.Id,
                    VoucherType = existingVoucher.VoucherType,
                    VoucherNumber = existingVoucher.VoucherNumber,
                    VoucherDate = existingVoucher.VoucherDate,
                    VoucherStatus= existingVoucher.VoucherStatus,
                    PaymentType = existingVoucher.PaymentType,
                    Narration = existingVoucher.Narration,
                    TotalAmount = existingVoucher.TotalAmount,
                    CompanyId = existingVoucher.CompanyId,
                    Status= "Delete",
                    EntryBy = entity.EntryBy,
                    EntryDate = DateTime.Now
                };
                _ICommonService.Add<AccVoucherLog>(voucherLog);

                foreach (var detail in existingDetail)
                {
                    var detailLog = new AccVoucherDetailsLog
                    {
                        VoucherDetailsId = detail.Id,
                        AccVoucherId = detail.AccVoucherId,
                        LedgerId = detail.LedgerId,
                        ShortDesc = detail.ShortDesc,
                        Amount = detail.Amount,
                        TranType = detail.TranType,
                        PaymentType = detail.PaymentType,
                        ChequeNo = detail.ChequeNo,
                        BankNbranch = detail.BankNbranch,
                        ChequeDate = detail.ChequeDate,
                        EntryBy = entity.EntryBy,
                        EntryDate = DateTime.Now,
                        Status = "Delete"
                    };
                    if (detail.TranType == "Dr")
                    {
                        detailLog.DebitAmount = detail.Amount;
                        detailLog.CreditAmount = 0;
                    }
                    else
                    {
                        detailLog.CreditAmount = detail.Amount;
                        detailLog.DebitAmount = 0;
                    }

                    _ICommonService.Add<AccVoucherDetailsLog>(detailLog);
                }
                // Fetch the AccVoucher along with its dependent voucherEntryDetails
                var voucher = _dbContext.voucherEntryDetails
                                        .Include(v => v.voucherEntryDetails)
                                        .FirstOrDefault(v => v.Id == entity.Id);

                if (voucher == null)
                {
                    throw new Exception("Voucher not found.");
                }

                // Remove dependent voucher details first
                if (voucher.voucherEntryDetails != null && voucher.voucherEntryDetails.Any())
                {
                    _dbContext.VoucherEntry.RemoveRange(voucher.voucherEntryDetails);
                }

                // Remove the parent voucher
                _dbContext.voucherEntryDetails.Remove(voucher);

                _dbContext.SaveChanges();

                return voucher.Id;
            }
            catch (Exception ex)
            {
                throw; 
            }
        }




        public int Update(AccVoucher entity)
        {
            try
            {
                _ICommonService.Update<AccVoucher>(entity);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        //public int UpdateOrAdd(AccVoucher voucher)
        //{
        //    try
        //    {

        //        if (voucher.Id > 0)
        //        {
        //            var existingVoucher = _ICommonService.GetWithNoTracking<AccVoucher>(voucher.Id);

        //            var existingDetail = _ICommonService
        //             .GetByCondition<VoucherDetails>(d => d.AccVoucherId == voucher.Id)
        //             .AsNoTracking()
        //             .ToList();

        //            var voucherLog = new AccVoucherLog
        //            {
                        
        //                VoucherId = existingVoucher.Id,
        //                VoucherType = existingVoucher.VoucherType,
        //                VoucherNumber= existingVoucher.VoucherNumber,
        //                VoucherDate = existingVoucher.VoucherDate,
        //                PaymentType = existingVoucher.PaymentType,
        //                Narration= existingVoucher.Narration,
        //                TotalAmount= existingVoucher.TotalAmount,
        //                VoucherStatus=existingVoucher.VoucherStatus,
        //                Status = "Edit",
        //                CompanyId = existingVoucher.CompanyId,
        //                UnitId=existingVoucher.UnitId,
        //                TeamId=existingVoucher.TeamId,
        //                EntryBy = voucher.UpdateBy,
        //                EntryDate = DateTime.Now
        //            };
        //            _ICommonService.Add<AccVoucherLog>(voucherLog);

        //            // Log the existing voucher details to VoucherDetailsLog
        //            foreach (var detail in existingDetail)
        //            {
        //                var detailLog = new AccVoucherDetailsLog
        //                {
        //                    VoucherDetailsId = detail.Id,
        //                    AccVoucherId = detail.AccVoucherId,
        //                    LedgerId = detail.LedgerId,
        //                    ShortDesc = detail.ShortDesc,
        //                    Amount = detail.Amount,
        //                    TranType = detail.TranType,
        //                    PaymentType = detail.PaymentType,
        //                    ChequeNo = detail.ChequeNo,
        //                    BankNbranch = detail.BankNbranch,
        //                    ChequeDate = detail.ChequeDate,
        //                    EntryBy = voucher.UpdateBy,
        //                    EntryDate = DateTime.Now,
        //                    Status = "Edit"
        //                };
        //                if (detail.TranType == "Dr")
        //                {
        //                    detailLog.DebitAmount = detail.Amount;
        //                }
        //                else
        //                {
        //                    detailLog.CreditAmount = detail.Amount;
        //                }

        //                _ICommonService.Add<AccVoucherDetailsLog>(detailLog);
        //            }

        //            _ICommonService.Update<AccVoucher>(voucher);


        //            foreach (var detail in voucher.voucherEntryDetails)
        //            {
        //                if (detail.Id > 0)
        //                {
        //                    // Update existing detail
        //                    _ICommonService.Update<VoucherDetails>(detail);
        //                }
        //                else
        //                {
        //                    // Add new detail
        //                    detail.AccVoucherId = voucher.Id; 
        //                    _ICommonService.Add<VoucherDetails>(detail);
        //                }
        //            }

                    
        //            // Remove details that are not in the current collection
        //            var inputDetailIds = voucher.voucherEntryDetails.Select(d => d.Id).Where(id => id > 0).ToList();
        //            var existingDetails = _ICommonService
        //            .GetByCondition<VoucherDetails>(d => d.AccVoucherId == voucher.Id)
        //            .ToList();
        //            var detailsToDelete = existingDetails.Where(d => !inputDetailIds.Contains(d.Id)).ToList();
        //            foreach (var detail in detailsToDelete)
        //            {
        //                _ICommonService.Remove<VoucherDetails>(detail);
        //            }
        //       }
        //        else
        //        {
        //            // Add a new voucher
        //            _ICommonService.Add<AccVoucher>(voucher);

        //            // Add all details linked to the new voucher
        //            foreach (var detail in voucher.voucherEntryDetails)
        //            {
        //                detail.AccVoucherId = voucher.Id; // Link the detail to the new voucher
        //                _ICommonService.Add<VoucherDetails>(detail);
        //            }
        //        }

        //        return voucher.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log and rethrow the exception for debugging purposes
        //        throw new Exception("An error occurred while saving the voucher and its details.", ex);
        //    }
        ////}


        public RptAccVoucher GetVocherById(int id)
        {
            try
            {
                RptAccVoucher voucher = new RptAccVoucher();
                List<RptVoucherDetails> details = new List<RptVoucherDetails>();
                AccVoucher accVoucher = _dbContext.voucherEntryDetails.Find(id);
                string query = "SELECT Il.Title AS LedgerName, v.* FROM AcctblVoucherDetails AS v JOIN AcctblILedger AS Il ON v.LedgerId = Il.Id;";
                details = _IDapperService.GetAllByQuery<RptVoucherDetails>(query).ToList();
                details = details.Where(x => x.AccVoucherId == id).ToList();
                voucher.Id = accVoucher.Id;
                voucher.VoucherNumber = accVoucher.VoucherNumber;   
                voucher.Narration = accVoucher.Narration;
                voucher.CompanyId = accVoucher.CompanyId;
                voucher.UnitId = accVoucher.UnitId;
                voucher.TeamId = accVoucher.TeamId;
                voucher.IsActive = accVoucher.IsActive;
                voucher.EntryBy = accVoucher.EntryBy;
                voucher.PaymentType = accVoucher.PaymentType;
                voucher.TotalAmount = accVoucher.TotalAmount;
                voucher.VoucherType= accVoucher.VoucherType;
                voucher.VoucherStatus = accVoucher.VoucherStatus;
                voucher.VoucherDate = accVoucher.VoucherDate;
                voucher.rptVoucherDetails = details;
                return voucher;
            }
            catch
            {
                throw;
            }
        }
        public List<RptAccVoucherListWithDetails> ACCGetAllVoucherListWithDetails(DateTime fromDate,DateTime toDate,string? vhoucherType,string? voucherNumber, int companyId)
        {
            try
            {
                if(companyId > 0)
                {
                    string query = "[SP_ACC_GetAllVoucherListWithDetails]";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@voucherType", vhoucherType);
                    p.Add("@voucherNumber", voucherNumber);
                    p.Add("@companyId", companyId);
                    p.Add("@QueryChecker", 1);
                    List<RptAccVoucherListWithDetails> voucherWithDetails = _IDapperService.GetAllBySP<RptAccVoucherListWithDetails>(query, p).ToList();

                    return voucherWithDetails;
                }
                else
                {
                    string query = "[SP_ACC_GetAllVoucherListWithDetails]";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@fromDate", fromDate);
                    p.Add("@toDate", toDate);
                    p.Add("@voucherType", vhoucherType);
                    p.Add("@voucherNumber", voucherNumber);
                    p.Add("@QueryChecker", 2);


                    List<RptAccVoucherListWithDetails> voucherWithDetails = _IDapperService.GetAllBySP<RptAccVoucherListWithDetails>(query, p).ToList();

                    return voucherWithDetails;
                }
              

            }
            catch 
            {
                throw;
            }
            
        }
        public List<string> GetVoucherNoByVoucherType(DateTime fromDate,DateTime toDate,string voucherType)
        {
            try
            {
                if (voucherType== "All")
                {
                    var list = _ICommonService.GetAll<AccVoucher>().Where(x => x.VoucherDate >= fromDate && x.VoucherDate <= toDate).Select(a => a.VoucherNumber).ToList();
                    return list;
                }
                else
                {
                    var list = _ICommonService.GetAll<AccVoucher>().Where(x => x.VoucherDate >= fromDate && x.VoucherDate <= toDate && x.VoucherType == voucherType).Select(a => a.VoucherNumber).ToList();
                    return list;
                }
             
            }
            catch
            {
                throw;
            }
        }

        public int UpdateOrAdd(AccVoucher entity)
        {
            throw new NotImplementedException();
        }
    }
}
