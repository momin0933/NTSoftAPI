using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class SettingManager : ISetting
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        private readonly string _connectionString;
        SQLConnectionString _connectionStringService = new SQLConnectionString();
        public SettingManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
            _connectionString = _connectionStringService.GetConnectionString("default");
        }
        public int Add(StockAdjustment entity)
        {
            var existingActiveRecord = _dbContext.StockAdjustments
                .FirstOrDefault(x => x.WhId == entity.WhId && x.IsActive==true);

            if (existingActiveRecord != null)
            {
                return existingActiveRecord.Id;
            }
            _ICommonService.Add(entity);
            return entity.Id;
        }


        public List<StockAdjustment> GetAll()
        {
            return _ICommonService.GetAll<StockAdjustment>().ToList();
        }

        public int Remove(StockAdjustment entity)
        {
            throw new NotImplementedException();
        }

        public int Update(StockAdjustment entity)
        {
            throw new NotImplementedException();
        }

        public int AddStockAdjustment(List<StockAdjustmentModel> entity)
        {
            foreach (var adj in entity)
            {
                // Save each adjustment entry to the database
                var adjustment = new StockAdjustmentModel
                {
                    AdjustmentId = adj.AdjustmentId,
                    ItemId = adj.ItemId,
                    VariantId = adj.VariantId,
                    Barcode = adj.Barcode,
                    Qty = adj.Qty,  
                    EntryBy=adj.EntryBy
                };

                _ICommonService.Add<StockAdjustmentModel>(adjustment);
            }
            return entity.Count;
        }

        public List<FinalStockAdjustment> GetAdjutmentItemWithActualStock(int WhId)
        {
            string SP = "Sp_PosStockAdjustment";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@WhId", WhId);

            List<FinalStockAdjustment> ItemList = _IDapperService.GetAllBySP<FinalStockAdjustment>(SP, p).ToList();
            return ItemList;
        }

        public int AddFinalAdjustment(List<FinalStockAdjustment> entity)
        {
            try
            {
                int firstAdjustmentId = entity.First().AdjustmentId;
                foreach (var adjust in entity)
                {
                    var finalAdjust = new FinalStockAdjustment
                    {
                        AdjustmentId= adjust.AdjustmentId,
                        ItemId= adjust.ItemId,
                        VariantId=adjust.VariantId,
                        Barcode= adjust.Barcode,
                        PhyQty=adjust.PhyQty,
                        StockQty=adjust.StockQty,
                        AdjustmentQty= adjust.AdjustmentQty,
                        Comment= adjust.Comment,
                        WhId= adjust.WhId,
                        EntryBy=adjust.EntryBy,
                        Remarks=adjust.Remarks
                    };
                    _ICommonService.Add<FinalStockAdjustment>(finalAdjust);
                }

                var updateQuery = @"UPDATE PosTblPhyAdj
                    SET Status = 'Stop', 
                        IsActive = 0,
                        UpdateBy = @UpdateBy,
                        UpdateDate = @UpdateDate
                    WHERE Id = @AdjustmentId;";

                _dbContext.Database.ExecuteSqlRaw(updateQuery,
                    new SqlParameter("@AdjustmentId", firstAdjustmentId),
                    new SqlParameter("@UpdateBy", entity.First().EntryBy),
                    new SqlParameter("@UpdateDate", DateTime.Now));


                foreach (var item in entity)
                {

                    var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == item.ItemId && x.VariantId == item.VariantId && x.WhId == item.WhId).FirstOrDefault();
                    if (stock != null)
                    {
                        stock.PrevQty = stock.StockQty;
                        stock.StockQty = item.PhyQty;
                      
                        stock.AdjQty = item.AdjustmentQty;
                        stock.UpdateBy = item.EntryBy;
                        stock.UpdateDate = DateTime.Now;
                    }
                    _ICommonService.Update<ItemStock>(stock);

                };

                foreach (var item in entity)
                {
                    //var updateQuery2 = @"UPDATE PosTblPhyAdjDetails SET
                    //                        IsActive = 0
                    //                    WHERE AdjustmentId = @AdjustmentId;";

                    //_dbContext.Database.ExecuteSqlRaw(updateQuery2,
                    //    new SqlParameter("@AdjustmentId", item.AdjustmentId));
                    var updateQuery2 = @"UPDATE PosTblPhyAdjDetails SET
                            IsActive = 0,
                            UpdateBy = @UpdateBy,
                            UpdateDate = @UpdateDate
                        WHERE AdjustmentId = @AdjustmentId;";

                    _dbContext.Database.ExecuteSqlRaw(updateQuery2,
                        new SqlParameter("@AdjustmentId", item.AdjustmentId),
                        new SqlParameter("@UpdateBy", item.EntryBy),
                        new SqlParameter("@UpdateDate", DateTime.Now));

                }
                ;
                return entity.Count;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public List<RptItem> GetVariantByBarcode(string barcode)
        {
            string SP = "Sp_PosStockAdjustment";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@barcode", barcode);
            List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).ToList();
            return ItemList;
        }

        public int InventoryAdjustment(InventoryAdj entity)
        {
            _ICommonService.Add<InventoryAdj>(entity);



            foreach (var item in entity.AdjDetails)
            {
                var receiveDetails = _dbContext.POStblItemRcvDetails
                        .Where(d => d.ItemId == item.ItemId
                                 && d.VariantId == item.VariantId
                                 && d.RemainingQty > 0
                                 && _dbContext.ItemReceives.Any(r => r.WhId == entity.WhId))
                        .OrderBy(d => d.Id)
                        .ToList();

                int qtyToIssue = (int)item.Qty;

                foreach (var receiveDetail in receiveDetails)
                {
                    if (qtyToIssue <= 0) break;

                    decimal availableQty = (decimal)receiveDetail.RemainingQty;
                    decimal issueQty = (qtyToIssue >= availableQty) ? availableQty : qtyToIssue;

                    // Update IssueQty
                    var updateQuery = @"UPDATE POStblItemRcvDetails
                    SET IssueQty = IssueQty + @IssueQty, 
                        RemainingQty = RemainingQty - @IssueQty
                    WHERE Id = @Id;";

                    _dbContext.Database.ExecuteSqlRaw(updateQuery,
                        new SqlParameter("@IssueQty", issueQty),
                        new SqlParameter("@Id", receiveDetail.Id));

                    qtyToIssue -= (int)issueQty;



                    var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == item.ItemId && x.VariantId == item.VariantId && x.WhId == entity.WhId).FirstOrDefault();
                    if (stock != null)
                    {

                        if (item.Qty < 0)
                        {
                            stock.ReturnQty = stock.ReturnQty == null ? item.Qty * (-1) : stock.ReturnQty + item.Qty * (-1);

                        }
                        else
                        {
                            stock.ReturnQty = stock.ReturnQty;
                            stock.IssueQty = stock.IssueQty == null ? item.Qty : stock.IssueQty + item.Qty;
                        }
                        stock.StockQty = (stock.ReceiveQty + stock.ReturnQty) - stock.IssueQty;
                    }
                    _ICommonService.Update<ItemStock>(stock);



                    POStblItemTransaction transaction = new POStblItemTransaction
                    {
                        ItemId = item.ItemId,
                        VariantId = item.VariantId,
                        WhId = (int)(entity.WhId != null ? entity.WhId : 0),
                        TranDt =(entity.AdjDate),
                        Qty = item.Qty,
                        Status = entity.Type,
                        Remarks = item.Remarks,
                        EntryBy = item.EntryBy,
                        EntryDate = item.EntryDate,
                        IsActive = item.IsActive
                    };
                    _ICommonService.Add<POStblItemTransaction>(transaction);
                }
            }
          
            return entity.Id;
        }
    }
}
