using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class CommitteeSmsMailManager : ICommitteeSmsMail
    {
        private readonly NTSoftDbContextFactory _dbFactory;

        public CommitteeSmsMailManager(NTSoftDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<int> CreateCommitteeSmsMail(CommitteeSmSMail committeeSmsMail, List<SmsMailWithMember> members)
        {
            await using var db = _dbFactory.CreateDbContext();
            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;
                const string defaultUser = "System";

                committeeSmsMail.EntryDate ??= now;
                committeeSmsMail.IsActive ??= true;
                committeeSmsMail.EntryBy ??= defaultUser;

                foreach (var member in members)
                {
                    member.EntryDate ??= now;
                    member.IsActive ??= true;
                    member.EntryBy ??= committeeSmsMail.EntryBy ?? defaultUser;
                }

                // EF sets SMSMailId through the navigation property
                committeeSmsMail.Members = members;

                await db.CommitteeSmSMails.AddAsync(committeeSmsMail);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return committeeSmsMail.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogSaveFailure("SAVE FAILED", ex);
                throw;
            }
        }

        public async Task<CommitteeSmSMail?> GetById(int id)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.CommitteeSmSMails
                .AsNoTracking()
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive == true);
        }

        public async Task<List<CommitteeSmSMail>> GetAll()
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.CommitteeSmSMails
                .AsNoTracking()
                .Include(c => c.Members)
                .Where(c => c.IsActive == true)
                .OrderByDescending(c => c.EntryDate)
                .ToListAsync();
        }

        public async Task<bool> Update(CommitteeSmSMail committeeSmsMail)
        {
            await using var db = _dbFactory.CreateDbContext();

            var existing = await db.CommitteeSmSMails.FindAsync(committeeSmsMail.Id);
            if (existing == null) return false;

            existing.MailSub = committeeSmsMail.MailSub;
            existing.Mail = committeeSmsMail.Mail;
            existing.MailAttatchPath = committeeSmsMail.MailAttatchPath;
            existing.SMS = committeeSmsMail.SMS;
            existing.Remarks = committeeSmsMail.Remarks;

            // existing is already tracked, no Update() call needed
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var committeeSmsMail = await db.CommitteeSmSMails.FindAsync(id);
            if (committeeSmsMail == null) return false;

            var now = DateTime.Now;
            committeeSmsMail.IsActive = false;
            committeeSmsMail.UpdateDate = now;

            var members = await db.SmsMailWithMembers
                .Where(m => m.SMSMailId == id)
                .ToListAsync();

            foreach (var member in members)
            {
                member.IsActive = false;
                member.UpdateDate = now;
            }

            return await db.SaveChangesAsync() > 0;
        }

        public async Task<int> CreateCustomerSmsMail(CustomerSmsMail customerSmsMail, List<SmsMailWithCustomer> members)
        {
            await using var db = _dbFactory.CreateDbContext();
            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;
                const string defaultUser = "System";

                customerSmsMail.EntryDate ??= now;
                customerSmsMail.IsActive ??= true;
                customerSmsMail.EntryBy ??= defaultUser;

                foreach (var member in members)
                {
                    member.EntryDate ??= now;
                    member.IsActive ??= true;
                    member.EntryBy ??= customerSmsMail.EntryBy ?? defaultUser;
                }

                customerSmsMail.Members = members;

                await db.CustomerSmSMails.AddAsync(customerSmsMail);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return customerSmsMail.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogSaveFailure("SMS MAIL SAVE FAILED", ex);
                throw;
            }
        }

        private static void LogSaveFailure(string title, Exception ex)
        {
            var rootEx = ex;
            while (rootEx.InnerException != null) rootEx = rootEx.InnerException;

            Console.WriteLine($"=== {title} ===");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Root Error: {rootEx.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}