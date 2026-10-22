using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class OwnerManager : IOwner
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _IDapperService;

        public OwnerManager(NTSoftDbContextFactory dbFactory, IDapperService dapperService)
        {
            _dbFactory = dbFactory;
            _IDapperService = dapperService;
        }

        public int Add(Owner entity)
        {
            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            try
            {
                var iLedger = db.AccLevelTwo.FirstOrDefault(x => x.Code == "24000")
                    ?? throw new Exception("Ledger with code '24000' not found.");

                // Owner + তার FlatOwners একসাথে সেভ
                db.Owners.Add(entity);
                db.SaveChanges();

                var flatOwners = entity.FlatOwners ?? new List<FlatOwner>();

                // প্রতিটা building-এর mosque bill count আপডেট
                var buildingIds = flatOwners
                    .Where(x => x.BuildingId.HasValue)
                    .Select(x => x.BuildingId!.Value)
                    .Distinct();

                foreach (var buildingId in buildingIds)
                    UpdateMosqueBillCount(db, buildingId);

                // প্রতিটা flat-এর জন্য owner ledger তৈরি
                foreach (var item in flatOwners)
                {
                    if (!item.FlatId.HasValue)
                        continue;

                    var flat = db.Flats.AsNoTracking().FirstOrDefault(x => x.Id == item.FlatId.Value)
                        ?? throw new Exception($"Flat not found for ID: {item.FlatId}");

                    db.AccLevelTwo.Add(new AccILedger
                    {
                        GLedgerId = iLedger.GLedgerId,
                        LdgAccNo = flat.FlatCode,
                        Code = "24" + flat.FlatCode,
                        Title = entity.Name,
                        ParentId = iLedger.Id,
                        OPBalance = iLedger.OPBalance,
                        PaymentType = iLedger.PaymentType,
                        EntryBy = item.EntryBy,
                        CompCode = "1"
                    });
                }

                db.SaveChanges();
                tx.Commit();

                return entity.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding owner: " + ex.Message, ex);
            }
        }

        public List<Owner> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Owners
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Include(x => x.FlatOwners)
                .ToList();
        }

        public IEnumerable<Owner> GetOwners(int Id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Owners
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.Id == Id)
                .Include(x => x.FlatOwners)
                .ToList();
        }

        public List<VwServiceCharge> GetFlatWiseServiceCharge(string year, string month)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@Year", year);
            p.Add("@Month", month);
            return _IDapperService.GetAllBySP<VwServiceCharge>("SP_GetServiceCharge", p).ToList();
        }

        public List<VwServiceCharge> GetFlatWiseOwnerList()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            return _IDapperService.GetAllBySP<VwServiceCharge>("SP_GetServiceCharge", p).ToList();
        }

        public VwServiceCharge GetFlatWiseServiceChargeDetails(string month, string year, int FCode)
        {
            const string SP = "SP_GetServiceCharge";

            var p = new DynamicParameters();
            p.Add("@QueryChecker", 3);
            p.Add("@month", month);
            p.Add("@year", year);
            p.Add("@FCode", FCode);

            var result = _IDapperService.GetByDynamicSPSingle<VwServiceCharge>(SP, p)
                ?? throw new Exception($"Service charge not found for FCode {FCode}, {month} {year}");

            var p1 = new DynamicParameters();
            p1.Add("@QueryChecker", 4);
            p1.Add("@month", month);
            p1.Add("@year", year);
            result.ExpenseList = _IDapperService.GetAllBySP<RptExpense>(SP, p1).ToList();

            var p2 = new DynamicParameters();
            p2.Add("@QueryChecker", 5);
            p2.Add("@month", month);
            p2.Add("@year", year);
            p2.Add("@FCode", FCode);
            result.GasExpenseList = _IDapperService.GetAllBySP<RptGasExpense>(SP, p2).ToList();

            var p3 = new DynamicParameters();
            p3.Add("@QueryChecker", 7);
            p3.Add("@month", month);
            p3.Add("@year", year);
            p3.Add("@FCode", FCode);
            result.ExtraChargeList = _IDapperService.GetAllBySP<RptExtraCharge>(SP, p3).ToList();

            return result;
        }

        public int Remove(Owner entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Owner entity)
        {
            throw new NotImplementedException();
        }

        public int UpdateOwner(VwServiceCharge entity)
        {
            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            var owner = db.Owners.FirstOrDefault(x => x.Id == entity.Id)
                ?? throw new Exception($"Owner not found. Id = {entity.Id}");

            owner.ImgPath = entity.ImgPath;
            owner.Name = entity.Name;
            owner.Phone = entity.Phone;
            owner.Email = entity.Email;
            owner.NID = entity.NID;
            owner.Religion = entity.Religion;
            owner.Gender = entity.Gender;
            owner.Address = entity.Address;
            owner.UpdateBy = entity.UpdateBy;
            owner.IsActive = true;

            var ledger = db.AccLevelTwo.FirstOrDefault(x => x.LdgAccNo == entity.FlatCode);
            if (ledger != null)
            {
                ledger.Title = entity.Name;
                ledger.UpdateBy = entity.UpdateBy;
            }

            db.SaveChanges();
            tx.Commit();

            return owner.Id;
        }

        public int UpdateTenant(VwServiceCharge entity)
        {
            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            var flatOwner = db.FlatOwners.FirstOrDefault(x => x.Id == entity.Id)
                ?? throw new Exception($"Flat owner not found. Id = {entity.Id}");

            flatOwner.TntName = entity.TntName;
            flatOwner.TntPhone = entity.TntPhone;
            flatOwner.TntEmail = entity.TntEmail;
            flatOwner.TntNID = entity.TntNID;
            flatOwner.TntReligion = entity.TntReligion;
            flatOwner.TntGender = entity.TntGender;
            flatOwner.TntAddress = entity.TntAddress;
            flatOwner.IsMosqueBillGenerate = entity.IsMosqueBillGenerate;
            flatOwner.UpdateBy = entity.UpdateBy;
            flatOwner.IsActive = true;

            db.SaveChanges();

            if (entity.BuildingId.HasValue)
                UpdateMosqueBillCount(db, entity.BuildingId.Value);

            db.SaveChanges();
            tx.Commit();

            return flatOwner.Id;
        }

        private static void UpdateMosqueBillCount(NTSoftDbContext db, int buildingId)
        {
            var building = db.Buildings.FirstOrDefault(x => x.Id == buildingId);
            if (building == null) return;

            building.MosqueBillGenerate = db.FlatOwners
                .Count(x => x.BuildingId == buildingId && x.IsMosqueBillGenerate == true);
        }
    }
}