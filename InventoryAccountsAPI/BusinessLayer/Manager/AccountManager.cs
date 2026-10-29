
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class AccountManager: IBankAccount
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        private readonly string _connectionString;
        SQLConnectionString _connectionStringService = new SQLConnectionString();

        public AccountManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
            _connectionString = _connectionStringService.GetConnectionString("default");
        }

        public async Task<POStblAccount> GetById(int id)
        {
            var accountList = await _dbContext.bankAccounts.FindAsync(id);
            return accountList;
        }

        public List<POStblAccount> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<POStblAccount>()
                    //.Where(x => x.IsActive == true)
                    .ToList();

            }
            catch
            {
                throw;
            }
        }

        //public int Add(POStblAccount entity)
        //{
        //    try
        //    {
        //        _ICommonService.Add<POStblAccount>(entity);
        //        return entity.Id;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public int Add(POStblAccount entity)
        {
            try
            {
                if (entity == null)
                    throw new Exception("Account data is null");

                //  Add Account
                _ICommonService.Add<POStblAccount>(entity);

                // Create Ledger only if AccountCode exists
                if (!string.IsNullOrEmpty(entity.AccCode))
                {
                    // Get Account group ledger (like Customer group)
                    var accountGroupLedger = _ICommonService
                        .GetAll<AccILedger>()
                        .FirstOrDefault(x => x.Title == "Accounts");

                    if (accountGroupLedger == null)
                        throw new Exception("Account group ledger not found");

                    //  Create Account Ledger
                    AccILedger accountLedger = new AccILedger
                    {
                        CompCode = accountGroupLedger.CompCode,
                        GLedgerId = accountGroupLedger.GLedgerId,
                        Code = entity.AccCode,
                        Title = entity.AccountName,   
                        ParentId = accountGroupLedger.Id,
                        OPBalance = 0,
                        PaymentType = "Cash",
                        EntryBy = entity.EntryBy,
                        EntryDate = DateTime.Now,
                        IsActive = true
                    };

                    _ICommonService.Add<AccILedger>(accountLedger);
                }

                return entity.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public int Update(POStblAccount entity)
        //{
        //    try
        //    {
        //        _ICommonService.Update<POStblAccount>(entity);
        //        return entity.Id;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public int Update(POStblAccount entity)
        {
            try
            {
                // 1️⃣ Update Account
                _ICommonService.Update<POStblAccount>(entity);

                // 2️⃣ Ledger Part
                if (!string.IsNullOrWhiteSpace(entity.AccCode))
                {
                    var accountGroupLedger = _ICommonService
                        .GetAll<AccILedger>()
                        .FirstOrDefault(x => x.Title == "Accounts");

                    if (accountGroupLedger != null)
                    {
                        var existingLedger = _ICommonService
                            .GetAll<AccILedger>()
                            .FirstOrDefault(x =>
                                x.ParentId == accountGroupLedger.Id &&
                                x.Code == entity.AccCode);

                        if (existingLedger == null)
                        {
                            // 🔹 Create New Ledger
                            var newLedger = new AccILedger
                            {
                                CompCode = accountGroupLedger.CompCode,
                                GLedgerId = accountGroupLedger.GLedgerId,
                                Code = entity.AccCode,
                                Title = entity.AccountName,
                                ParentId = accountGroupLedger.Id,
                                OPBalance = 0,
                                PaymentType = "Cash",
                                EntryBy = entity.UpdateBy,
                                EntryDate = DateTime.Now,
                                IsActive = true
                            };

                            _ICommonService.Add(newLedger);
                        }
                        else
                        {
                            // 🔹 Update Existing Ledger
                            existingLedger.Title = entity.AccountName;
                            existingLedger.UpdateBy = entity.UpdateBy;
                            existingLedger.UpdateDate = DateTime.Now;

                            _ICommonService.Update(existingLedger);
                        }
                    }
                }

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Remove(POStblAccount entity)
        {
            try
            {
                //_ICommonService.RemoveById<POStblAccount>(entity.Id);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ToggleStatus(int id, bool isActive)
        {
            var account = await _dbContext.bankAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account == null) return false;

            account.IsActive = isActive;
            account.UpdateDate = DateTime.Now;

            _dbContext.bankAccounts.Update(account);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
