using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class GasManager : IGas
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _IDapperService;

        public GasManager(NTSoftDbContextFactory dbFactory, IDapperService dapperService)
        {
            _dbFactory = dbFactory;
            _IDapperService = dapperService;
        }

        public int Add(Gas entity)
        {
            using var db = _dbFactory.CreateDbContext();

            if (entity.Id == 0)
                db.Gases.Add(entity);
            else
                db.Gases.Update(entity);

            db.SaveChanges();
            return entity.Id;
        }

        public List<Gas> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Supplier> GetSuppliers()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Suppliers
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .ToList();
        }

        public int Remove(Gas entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Gases.Remove(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(Gas entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.Gases.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public List<RptGas> GetGasWithSupplier(DateTime fromDt, DateTime toDt)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 4);
            p.Add("@fromDt", fromDt);
            p.Add("@toDt", toDt);
            return _IDapperService.GetAllBySP<RptGas>("Sp_VbnExpense", p).ToList();
        }

        public List<Gas> GetGasById(int id)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 5);
            p.Add("@Id", id);
            return _IDapperService.GetAllBySP<Gas>("Sp_VbnExpense", p).ToList();
        }

        public List<RptGasUsage> GetGasUsage(string FCode)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@FlatCode", FCode);
            return _IDapperService.GetAllBySP<RptGasUsage>("Sp_GasInfo", p).ToList();
        }

        public int AddUsedGas(GasUsed entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.GasUseds.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int UpdateUsedGas(GasUsed entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.GasUseds.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        // ---------------- Gas Stock ----------------

        public List<RptGasStock> GetGasStock(DateTime fromDate, DateTime toDate)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@FromDt", fromDate);
            p.Add("@ToDt", toDate);
            return _IDapperService.GetAllBySP<RptGasStock>("Sp_GasInfo", p).ToList();
        }

        public int AddGasStock(GasStock entity)
        {
            using (var db = _dbFactory.CreateDbContext())
            {
                var existGasStock = db.GasStocks
                    .AsNoTracking()
                    .FirstOrDefault(x => x.FromDate == entity.FromDate
                                      && x.ToDate == entity.ToDate
                                      && x.BuildingId == entity.BuildingId);

                if (existGasStock != null)
                {
                    entity.Id = existGasStock.Id;
                    db.GasStocks.Update(entity);
                }
                else
                {
                    db.GasStocks.Add(entity);
                }

                db.SaveChanges();
            }

            // সেভ হওয়ার পরে stock recalculation SP
            _IDapperService.GetAllBySP<GasStockCal>("SP_GasStock", new DynamicParameters()).FirstOrDefault();

            return entity.Id;
        }

        public int UpdateGasStock(GasStock entity)
        {
            using var db = _dbFactory.CreateDbContext();

            // আগের মতোই: সবশেষ stock row আপডেট হয়
            var stock = db.GasStocks.OrderByDescending(g => g.Id).FirstOrDefault()
                ?? throw new Exception("No gas stock record found.");

            stock.OpeningQty = entity.OpeningQty;
            stock.RcvQty = entity.RcvQty;
            stock.Stock = entity.Stock;
            stock.WastageQty = entity.WastageQty;
            stock.FinalPrice = entity.FinalPrice;
            stock.UsageQty = entity.UsageQty;

            db.SaveChanges();
            return stock.Id;
        }

        public GasStockCal GetGasOpeningStock(int buildingId)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 3);
            p.Add("@BuildingId", buildingId);
            return _IDapperService.GetAllBySP<GasStockCal>("Sp_GasInfo", p).FirstOrDefault();
        }

        public List<RptGasUsage> GetAllFlatInfo()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 4);
            return _IDapperService.GetAllBySP<RptGasUsage>("Sp_GasInfo", p).ToList();
        }

        public int AddGasUsed(List<GasUsed> entity)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.GasUseds.UpdateRange(entity);
                db.SaveChanges();
                return entity.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<RptGasUsage> GetGasUsageList()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            return _IDapperService.GetAllBySP<RptGasUsage>("SP_GasUsed", p).ToList();
        }

        // ---------------- Closing declaration ----------------

        public int AddClosingDate(ClosingDeclaration entity)
        {
            if (!entity.ToDate.HasValue)
                throw new ArgumentException("ToDate cannot be null.");

            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            var previousMonthDate = entity.ToDate.Value.AddMonths(-1);
            var year = previousMonthDate.Year.ToString();
            var month = previousMonthDate.ToString("MMMM");

            var gasStocks = db.GasStocks
                .AsNoTracking()
                .Where(x => x.Year == year && x.Month == month)
                .OrderBy(x => x.BuildingId)
                .ToList();

            foreach (var stock in gasStocks)
            {
                var existingClosings = db.ClosingDeclarations
                    .Where(x => x.BuildingId == stock.BuildingId)
                    .ToList();

                DateTime? lastToDate = existingClosings
                    .OrderByDescending(x => x.ToDate)
                    .FirstOrDefault()?.ToDate;

                foreach (var closing in existingClosings)
                {
                    closing.IsActive = false;
                    closing.UpdateBy = entity.EntryBy;
                }

                var newFromDate = lastToDate.HasValue
                    ? lastToDate.Value.AddDays(1)
                    : entity.ToDate.Value.AddMonths(-1).AddDays(1);

                db.ClosingDeclarations.Add(new ClosingDeclaration
                {
                    BuildingId = stock.BuildingId,
                    FromDate = newFromDate,
                    ToDate = entity.ToDate,
                    DueCollectionDate = entity.DueCollectionDate,
                    OpeningQty = stock.Stock,
                    Year = entity.Year,
                    Month = entity.Month,
                    EntryBy = entity.EntryBy
                });

                db.SaveChanges(); // প্রতি building-এর পরে সেভ, আগের মতো
            }

            tx.Commit();
            return entity.Id;
        }

        public List<ClosingDeclaration> GetClosingDate()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 6);
            return _IDapperService.GetAllBySP<ClosingDeclaration>("Sp_GasInfo", p).ToList();
        }

        public int UpdateClosingDate(ClosingDeclaration entity)
        {
            using var db = _dbFactory.CreateDbContext();

            var list = db.ClosingDeclarations.Where(x => x.IsActive == true).ToList();
            foreach (var item in list)
            {
                item.ToDate = entity.ToDate;
                item.UpdateBy = entity.UpdateBy;
            }

            db.SaveChanges();
            return entity.Id;
        }
    }
}