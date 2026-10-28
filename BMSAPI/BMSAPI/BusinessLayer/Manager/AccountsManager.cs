using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class AccountsManager : IAccounts
    {
        private readonly NTSoftDbContextFactory _dbFactory;

        public AccountsManager(NTSoftDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<AccGLedger> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Accounts.AsNoTracking().Where(x => x.IsActive == true).ToList();
        }

        public AccGLedger? Get(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<AccGLedger?> GetById(int id)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public int Add(AccGLedger entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Accounts.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(AccGLedger entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Accounts.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Remove(AccGLedger entity)
        {
            using var db = _dbFactory.CreateDbContext();

            var account = db.Accounts.FirstOrDefault(x => x.Id == entity.Id)
                ?? throw new Exception($"Account not found. Id = {entity.Id}");

            // Soft delete: এই group-এর নিচে ledger আর voucher থাকতে পারে
            account.IsActive = false;
            account.UpdateBy = entity.UpdateBy;
            db.SaveChanges();

            return account.Id;
        }
    }
}