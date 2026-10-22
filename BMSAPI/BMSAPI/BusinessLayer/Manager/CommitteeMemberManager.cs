using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class CommitteeMemberManager : ICommitteeMember
    {
        private readonly NTSoftDbContextFactory _dbFactory;

        public CommitteeMemberManager(NTSoftDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public int AddCommitteeMembers(int cId, List<CommitteeMember> members)
        {
            using var db = _dbFactory.CreateDbContext();

            foreach (var member in members)
            {
                member.CId = cId;
                member.EntryBy ??= "System";
                // EntryDate আর IsActive SaveChanges override নিজেই সেট করবে
            }

            db.CommitteeMembers.AddRange(members);
            return db.SaveChanges();
        }

        public int Add(CommitteeMember entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.CommitteeMembers.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(CommitteeMember entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.CommitteeMembers.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Remove(CommitteeMember entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.CommitteeMembers.Remove(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public List<CommitteeMember> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.CommitteeMembers.AsNoTracking().ToList();
        }

        public CommitteeMember? Get(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.CommitteeMembers.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }
    }
}