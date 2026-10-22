using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class BuildingManager : IBuilding
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _IDapperService;

        public BuildingManager(NTSoftDbContextFactory dbFactory, IDapperService dapperService)
        {
            _dbFactory = dbFactory;
            _IDapperService = dapperService;
        }

        public int Add(Building entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Buildings.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int AddExpenseBill(Expenses entity)
        {
            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            if (entity.Id == 0)
                db.Expenses.Add(entity);
            else
                db.Expenses.Update(entity);

            db.SaveChanges();

            var expenseList = db.Expenses
                .Where(x => x.Month == entity.Month && x.Year == entity.Year)
                .ToList();

            var existingServiceCharge = db.ServiceCharges
                .FirstOrDefault(x => x.Month == entity.Month && x.Year == entity.Year);

            var buildings = db.Buildings.ToList();
            var totFlat = buildings.Sum(x => x.TotalFlats);
            var totalMosqueUsingFlat = buildings.Sum(x => x.MosqueBillGenerate);

            var totExpense = expenseList
                .Where(x => x.ExpenseType != "Mosque Electric Bill")
                .Sum(x => x.Amount) ?? 0;
            var totMosqExpense = expenseList
                .Where(x => x.ExpenseType == "Mosque Electric Bill")
                .Sum(x => x.Amount) ?? 0;

            if (existingServiceCharge != null)
            {
                existingServiceCharge.TotalFlat = totFlat;
                existingServiceCharge.TotalFlatUsingMosque = totalMosqueUsingFlat;
                existingServiceCharge.TotalExpense = totExpense;
                existingServiceCharge.FlatWiseExpense = totFlat > 0 ? totExpense / totFlat : 0;
                existingServiceCharge.TotalFlatUsingMosqueBill = totMosqExpense;
                existingServiceCharge.FlatWiseMosqueBill = totalMosqueUsingFlat > 0 ? totMosqExpense / totalMosqueUsingFlat : 0;
                existingServiceCharge.FlatWiseServiceCharge = existingServiceCharge.FlatWiseExpense + existingServiceCharge.FlatWiseMosqueBill;
                existingServiceCharge.UpdateBy = entity.UpdateBy;
                existingServiceCharge.IsActive = true;
            }
            else
            {
                var service = new ServiceCharge
                {
                    Month = entity.Month,
                    Year = entity.Year,
                    TotalFlat = totFlat,
                    TotalFlatUsingMosque = totalMosqueUsingFlat,
                    TotalExpense = totExpense,
                    FlatWiseExpense = totFlat > 0 ? totExpense / totFlat : 0,
                    TotalFlatUsingMosqueBill = totMosqExpense,
                    FlatWiseMosqueBill = totalMosqueUsingFlat > 0 ? totMosqExpense / totalMosqueUsingFlat : 0
                };
                service.FlatWiseServiceCharge = service.FlatWiseExpense + service.FlatWiseMosqueBill;
                db.ServiceCharges.Add(service);
            }

            db.SaveChanges();
            tx.Commit();

            return entity.Id;
        }

        public List<Building> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();

            var assignedFlatIds = db.FlatOwners
                .Where(f => f.FlatId.HasValue)
                .Select(f => f.FlatId!.Value)
                .ToHashSet();

            var buildings = db.Buildings
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Include(x => x.Flats)
                .ToList();

            foreach (var building in buildings)
            {
                building.Flats = (building.Flats ?? new List<Flats>())
                    .Where(f => !assignedFlatIds.Contains(f.Id))
                    .ToList();
            }

            return buildings;
        }

        public Building? Get(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Buildings.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public List<Building> GetAllBuildingFlatOwner()
        {
            using var db = _dbFactory.CreateDbContext();

            var buildings = db.Buildings
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Include(x => x.Flats)
                .Include(x => x.FlatOwners)
                .ToList();

            foreach (var building in buildings)
            {
                var owners = building.FlatOwners ?? new List<VwFlatOwner>();

                var ownedFlats = owners
                    .Select(o => $"{o.Floor}|{o.FlatNumber}")
                    .ToHashSet();

                var vacantFlats = (building.Flats ?? new List<Flats>())
                    .Where(f => !ownedFlats.Contains($"{f.Floor}|{f.FlatNumber}"))
                    .Select(f => new VwFlatOwner
                    {
                        Id = f.Id,
                        BuildingId = building.Id,
                        Floor = f.Floor,
                        FlatNumber = f.FlatNumber,
                        FlatCode = f.FlatCode,
                        FlatType = f.FlatType,
                        SizeSqft = f.SizeSqft
                    });

                building.FlatOwners = owners.Concat(vacantFlats).ToList();
            }

            return buildings;
        }

        public List<RptExpense> GetAllExpenses(DateTime? fromDate, DateTime? toDate)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@fromDt", fromDate);
            p.Add("@toDt", toDate);
            return _IDapperService.GetAllBySP<RptExpense>("Sp_VbnExpense", p).ToList();
        }

        public List<RptDashboard> GetDashboardData()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 3);
            return _IDapperService.GetAllBySP<RptDashboard>("Sp_VbnExpense", p).ToList();
        }

        public List<RptExpense> GetExpenseById(int id)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@Id", id);
            return _IDapperService.GetAllBySP<RptExpense>("Sp_VbnExpense", p).ToList();
        }

        public int Remove(Building entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Buildings.Remove(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(Building entity)
        {
            using var db = _dbFactory.CreateDbContext();

            var building = db.Buildings.FirstOrDefault(x => x.Id == entity.Id)
                ?? throw new Exception($"Building not found. Id = {entity.Id}");

            building.Name = entity.Name;
            building.Address = entity.Address;
            building.Location = entity.Location;
            building.UpdateBy = entity.UpdateBy;

            db.SaveChanges();
            return building.Id;
        }

        public int AddFlat(Flats entity)
        {
            using var db = _dbFactory.CreateDbContext();

            db.Flats.Add(entity);
            db.SaveChanges();

            UpdateBuildingFlatCount(db, entity.BuildingId);
            return entity.Id;
        }

        public int UpdateFlat(Flats entity)
        {
            using var db = _dbFactory.CreateDbContext();

            var flat = db.Flats.FirstOrDefault(f => f.Id == entity.Id)
                ?? throw new Exception($"Flat not found. Id = {entity.Id}");

            flat.Floor = entity.Floor;
            flat.FlatNumber = entity.FlatNumber;
            flat.FlatType = entity.FlatType;
            flat.SizeSqft = entity.SizeSqft;
            flat.FlatCode = entity.FlatCode;
            flat.UpdateBy = entity.UpdateBy;
            flat.IsActive = entity.IsActive;

            db.SaveChanges();

            UpdateBuildingFlatCount(db, flat.BuildingId);
            return flat.Id;
        }

        public int RemoveFlat(Flats entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Flats.Remove(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int DeleteExpense(Expenses expenses)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Expenses.Remove(expenses);
            db.SaveChanges();
            return expenses.Id;
        }

        private static void UpdateBuildingFlatCount(NTSoftDbContext db, int? buildingId)
        {
            if (buildingId == null) return;

            var building = db.Buildings.FirstOrDefault(x => x.Id == buildingId);
            if (building == null) return;

            building.TotalFlats = db.Flats.Count(x => x.BuildingId == buildingId);
            db.SaveChanges();
        }
    }
}