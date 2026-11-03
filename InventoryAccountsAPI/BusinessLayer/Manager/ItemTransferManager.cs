using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ItemTransferManager : IItemTransfer
    {

        //readonly FTDbContext _dbContext;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //public ItemTransferManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public ItemTransferManager(NTSoftDbContextFactory dbContext, IDapperService dapperService, ILogger<ItemManager> logger)
        {

            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
        }

        //public int Add(ItemTransfer entity)
        //{
        //    try
        //    {
        //        _ICommonService.Add<ItemTransfer>(entity);

        //        if (entity.ItemTransferList != null)
        //        {
        //            foreach (var itemTransferDetail in entity.ItemTransferList)
        //            {
        //                if (itemTransferDetail.ItemTransfer == null)
        //                {
        //                    itemTransferDetail.ItemTransfer = entity;
        //                }

        //                // Insert into POStblItemTransaction
        //                var sqlQuery = @"INSERT INTO POStblItemTransaction 
        //                        (VariantId, ItemId, Qty, WhId, TranDt, Status, Remarks, EntryDate, EntryBy, IsActive)
        //                        VALUES (@VariantId, @ItemId, @Qty, @WhId, GETDATE(), 'Transfer', @Remarks, GETDATE(), @EntryBy, 1);";

        //                _dbContext.Database.ExecuteSqlRaw(sqlQuery,
        //                    new SqlParameter("@VariantId", itemTransferDetail.VariantId),
        //                    new SqlParameter("@ItemId", itemTransferDetail.ItemId),
        //                    new SqlParameter("@Qty", itemTransferDetail.Qty),
        //                    new SqlParameter("@WhId", entity.WhId),
        //                    new SqlParameter("@Remarks", itemTransferDetail.Remarks ?? (object)DBNull.Value),
        //                    new SqlParameter("@EntryBy", entity.EntryBy));

        //                var receiveDetails = _dbContext.POStblItemRcvDetails
        //            .Where(d => d.ItemId == itemTransferDetail.ItemId
        //                     && d.VariantId == itemTransferDetail.VariantId
        //                       && d.Status == "Received"
        //                     && d.RemainingQty > 0
        //                     && _dbContext.ItemReceives.Any(r => r.WhId == entity.WhId))
        //            .OrderBy(d => d.Id) // FIFO method
        //            .ToList();


        //                int qtyToIssue = (int)itemTransferDetail.Qty;

        //                foreach (var receiveDetail in receiveDetails)
        //                {
        //                    if (qtyToIssue <= 0) break;

        //                    decimal availableQty = (decimal)receiveDetail.RemainingQty;
        //                    decimal issueQty = (qtyToIssue >= availableQty) ? availableQty : qtyToIssue;

        //                    // Update IssueQty
        //                    var updateQuery = @"UPDATE POStblItemRcvDetails
        //                                SET IssueQty = IssueQty + @IssueQty, 
        //                                    RemainingQty = RemainingQty - @IssueQty
        //                                WHERE Id = @Id;";

        //                    _dbContext.Database.ExecuteSqlRaw(updateQuery,
        //                        new SqlParameter("@IssueQty", issueQty),
        //                        new SqlParameter("@Id", receiveDetail.Id));

        //                    qtyToIssue -= (int)issueQty;
        //                }
        //            }
        //        }

        //        return entity.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}




        public string UpdateStockAndItem(ItemTransfer entity)
        {
            try
            {
                var itemList = entity.ItemTransferList;
                foreach (var dt in itemList)
                {
                    var Fromstock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == dt.ItemId && x.WhId == entity.WhId && x.VariantId==dt.VariantId).FirstOrDefault();
                    if (Fromstock != null)
                    {

                        Fromstock.StockQty = Fromstock.StockQty - dt.Qty;
                        
                        Fromstock.IssueQty=  Fromstock.IssueQty+dt.Qty;
                        
                       
                    }
                    _ICommonService.Update<ItemStock>(Fromstock);
                    
                }
                return "Stock Updated";
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<ItemTransfer> GetAll()
        {
            try
            {
                List<ItemTransfer> transfersList = new List<ItemTransfer>();
                transfersList = _ICommonService.GetAll<ItemTransfer>().DistinctBy(x=>x.TransferNo).ToList();
                return transfersList;
            }
            catch
            {
                throw;
            }
            
        }
        public List<ItemTransfer> GetAllTransferItemByWareHouse(int whId)
        {
            try 
            {
                string SP = "SP_GetAllTransferItemByWareHouse";
                DynamicParameters p = new DynamicParameters();
                p.Add("@WhId", whId);
                List<ItemTransfer> itemTransfer = _IDapperService.GetAllBySP<ItemTransfer>(SP, p).ToList();

                return itemTransfer;

            }
            catch
            {
                throw;
            }
            
        }

        public int Remove(ItemTransfer entity)
        {
            throw new NotImplementedException();
        }

        public int Update(ItemTransfer entity)
        {

            throw new NotImplementedException();
        }
        public List<ItemTransferWithDetails> GetItemTransferWithDetails(string transferNo)
        {
            try
            {
                string SP = "SP_GetItemTransferDetails";
                DynamicParameters p = new DynamicParameters();
                p.Add("@TrannsferNo", transferNo);
                List<ItemTransferWithDetails> itemTransferWithDetailsList = _IDapperService.GetAllBySP<ItemTransferWithDetails>(SP, p).ToList();

                return itemTransferWithDetailsList;
            }
            catch
            {
                throw;
            }
        }


        //public int UpdateTransferItemDetails(ItemTransfer entity)
        //{
        //    try
        //    {
        //        //_ICommonService.Add(entity);
        //        int isActive = 0;

        //        if (entity != null)
        //        {
        //            isActive = Convert.ToInt32(entity.IsActive);
        //        }



        //        foreach (var dt in entity.ItemTransferList)
        //        {
        //            string query = $"UPDATE POStblItemTransferDetails SET RcvQty = {dt.RcvQty}, Status = '{dt.Status}', UpdateDate = '{dt.UpdateDate}', UpdateBy = '{dt.UpdateBy}', IsActive = {1} WHERE id = {dt.Id};";
        //            _IDapperService.UpdateByQuery(query);

        //            var fromStock = _ICommonService.GetAll<ItemStock>().FirstOrDefault(x => x.VariantId == dt.VariantId && x.WhId==entity.WhId && x.ItemId==dt.ItemId);
        //            if (fromStock != null)
        //            {
        //                fromStock.StockQty += dt.Qty;
        //                fromStock.ReceiveQty += dt.Qty;
        //                fromStock.UpdateBy = dt.UpdateBy;
        //                _ICommonService.Update<ItemStock>(fromStock);
        //            }
        //        }

        //        string query1 = $"UPDATE POStblItemTransfer SET Status = '{entity.Status}', UpdateDate= '{entity.UpdateDate}', UpdateBy = '{entity.UpdateBy}', IsActive = {1} where TransferNo = '{entity.TransferNo}';";

        //        _IDapperService.UpdateByquery(query1);



        //        string sqlQueryTransaction = @"INSERT INTO POStblItemTransaction 
        //        (ItemId,VariantId, WhId, TranDt, Qty, Status, Remarks, EntryDate, EntryBy, IsActive)
        //        VALUES (@ItemId,@VariantId, @WhId, GETDATE(), @Qty, @status, @Remarks, GETDATE(), @EntryBy, 1);";
        //        foreach (var dt in entity.ItemTransferList)
        //        {
        //            _dbContext.Database.ExecuteSqlRaw(sqlQueryTransaction,
        //                new SqlParameter("@ItemId", dt.ItemId),
        //                new SqlParameter("@VariantId", dt.VariantId),
        //                new SqlParameter("@WhId", entity.WhId),
        //                new SqlParameter("@Qty", dt.Qty),
        //                new SqlParameter("@status", dt.Status),
        //                new SqlParameter("@Remarks", dt.Remarks ?? (object)DBNull.Value),
        //                new SqlParameter("@EntryBy", entity.EntryBy));
        //        }
        //        string SP = "SP_PosInsertTransferRcv";
        //        DynamicParameters p = new DynamicParameters();
        //        p.Add("@WhId", entity.WhId);
        //        p.Add("@ReceiveNo", entity.TransferNo);
        //        p.Add("@TotalQty", entity.TotalQty);
        //        p.Add("@Remarks", entity.Remarks);
        //        p.Add("@Status", entity.Status);
        //        p.Add("@EntryBy", entity.EntryBy);
        //        p.Add("@RcvId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //        // Retrieve the rcvId
        //        var rcvId = _IDapperService.GetByDynamicSPSingle<ReveiveId>(SP, p);

        //        // Retrieve the actual value from the parameter
        //        var rcvIdValue = p.Get<int>("@RcvId");

        //        var insertRcvDetailsQuery = @"INSERT INTO POStblItemRcvDetails 
        //(RcvId, ItemId, VariantId, Qty, IssueQty, RemainingQty,Status, Remarks,CostingPrice,SellingPrice, EntryDate, EntryBy, IsActive)
        //VALUES (@RcvId, @ItemId, @VariantId, @Qty, @IssueQty, @RemainingQty,'Transfer-Received', @Remarks,@CostingPrice,@SellingPrice, GETDATE(), @EntryBy, 1);";

        //        decimal? sellingPrice = 0;
        //        decimal? costingPrice = 0;
        //        int? issueQty = 0;
        //        foreach (var dt in entity.ItemTransferList)
        //        {
        //            sellingPrice = _ICommonService.GetAll<POStblItemRcvDetails>()
        //                .Where(x => x.ItemId == dt.ItemId && x.VariantId == dt.VariantId && x.Status == "Received")
        //                .OrderByDescending(x => x.Id) // Assuming higher Id means latest entry
        //                .FirstOrDefault()?.SellingPrice;

        //            costingPrice = _ICommonService.GetAll<POStblItemRcvDetails>()
        //                .Where(x => x.ItemId == dt.ItemId && x.VariantId == dt.VariantId && x.Status == "Received")
        //                .OrderByDescending(x => x.Id) // Sorting to get the last received record
        //                .FirstOrDefault()?.CostingPrice;

        //            _dbContext.Database.ExecuteSqlRaw(insertRcvDetailsQuery,
        //            new SqlParameter("@RcvId", rcvIdValue),
        //            new SqlParameter("@ItemId", dt.ItemId),
        //            new SqlParameter("@VariantId", dt.VariantId),
        //            new SqlParameter("@Qty", dt.Qty),
        //            new SqlParameter("@RemainingQty", dt.Qty),
        //            new SqlParameter("@IssueQty", issueQty),
        //            new SqlParameter("@CostingPrice", SqlDbType.Decimal) { Value = costingPrice },
        //            new SqlParameter("@SellingPrice", SqlDbType.Decimal) { Value = sellingPrice },
        //            new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
        //            new SqlParameter("@EntryBy", entity.EntryBy));
        //        }




        //        return entity.Id;
        //    }

        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while updating the transfer item details", ex);
        //    }
        //}






        //public int UpdateTransferItemDetails(ItemTransferReceive entity)
        //{
        //    try
        //    {
        //        int isActive = 0;

        //        if (entity != null)
        //        {
        //            isActive = Convert.ToInt32(entity.IsActive);
        //        }

        //        // Update POStblItemTransferDetails
        //        string query = $"UPDATE POStblItemTransferDetails SET Qty = {entity.Qty}, Status = '{entity.Status}', UpdateDate = '{entity.UpdateDate}', UpdateBy = '{entity.UpdateBy}', IsActive = {isActive} WHERE Id = {entity.Id};";
        //        _IDapperService.UpdateByQuery(query);

        //        // Update stock quantities for the main item
        //        var fromStock = _ICommonService.GetAll<ItemStock>().FirstOrDefault(x => x.ItemId == entity.ItemId);
        //        if (fromStock != null)
        //        {
        //            fromStock.StockQty += entity.Qty;
        //            fromStock.ReceiveQty += entity.Qty;
        //            fromStock.UpdateBy = entity.UpdateBy;
        //            _ICommonService.Update<ItemStock>(fromStock);
        //        }

        //        // Insert into POStblItemTransaction table
        //        var sqlQueryTransaction = @"INSERT INTO FashionTexLive.dbo.POStblItemTransaction 
        //(ItemId, WhId, TranDt, Qty, Status, Remarks, EntryDate, EntryBy, IsActive)
        //VALUES (@ItemId, @WhId, GETDATE(), @Qty, 'Shop-Received', @Remarks, GETDATE(), @EntryBy, 1);";

        //        _dbContext.Database.ExecuteSqlRaw(sqlQueryTransaction,
        //            new SqlParameter("@ItemId", entity.ItemId),
        //            new SqlParameter("@WhId", fromStock.WhId),
        //            new SqlParameter("@Qty", entity.Qty),
        //            new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
        //            new SqlParameter("@EntryBy", entity.UpdateBy));



        //        string SP = "SP_PosInsertTransferRcv";
        //        DynamicParameters p = new DynamicParameters();
        //        p.Add("@WhId", fromStock.WhId);
        //        p.Add("@ReceiveNo", entity.TransferNo);
        //        p.Add("@TotalQty", entity.TotalQty);
        //        p.Add("@Remarks", entity.Remarks);
        //        p.Add("@EntryBy", entity.UpdateBy);
        //        p.Add("@RcvId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //        // Retrieve the rcvId
        //        var rcvId = _IDapperService.GetByDynamicSPSingle<ReveiveId>(SP, p);

        //        // Retrieve the actual value from the parameter
        //        var rcvIdValue = p.Get<int>("@RcvId");

        //        var insertRcvDetailsQuery = @"INSERT INTO FashionTexLive.dbo.POStblItemRcvDetails 
        //(RcvId, ItemId, Qty, Status, Remarks,CostingPrice,SellingPrice, EntryDate, EntryBy, IsActive)
        //VALUES (@RcvId, @ItemId, @Qty, 'Transfer-Received', @Remarks,@CostingPrice,@SellingPrice, GETDATE(), @EntryBy, 1);";


        //        _dbContext.Database.ExecuteSqlRaw(insertRcvDetailsQuery,
        //            new SqlParameter("@RcvId", rcvIdValue),
        //            new SqlParameter("@ItemId", entity.ItemId),
        //            new SqlParameter("@Qty", entity.Qty),
        //         new SqlParameter("@CostingPrice", SqlDbType.Decimal) { Value = entity.CostingPrice },
        //         new SqlParameter("@SellingPrice", SqlDbType.Decimal) { Value = entity.SellingPrice },
        //            new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
        //            new SqlParameter("@EntryBy", entity.UpdateBy));
        //        return entity.Id;
        //    }

        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while updating the transfer item details", ex);
        //    }
        //}



        public List<RptItemTransfer> GetAllReceiveTransferItem(int whId,DateTime FromDt, DateTime ToDt, string Status)
        {
            try
            {
                string SP = "SP_GetTransferItem";
                DynamicParameters p = new DynamicParameters();

                if(Status == "'Receive'")
                {
                    p.Add("@QueryChecker", 1);
                }
                else if(Status == "'Transfer'")
                {
                    p.Add("@QueryChecker", 2);
                }
               
                p.Add("@whId", whId);
                p.Add("@FromDt", FromDt);
                p.Add("@ToDt", ToDt);
                List<RptItemTransfer> list = _IDapperService.GetAllBySP<RptItemTransfer>(SP, p).ToList();

                foreach (var item in list)
                {
                    Console.WriteLine($"CompanyName: {item.CompanyName}");
                }
                return list;
            }
            catch
            {
                throw;
            }
        }

        public List<ItemNameList> GetReceiveItemByReceiveNo(string ReceiveNo)
        {
            string SP = "SP_GetReceiveNowiseValue";
            DynamicParameters p = new DynamicParameters();
            p.Add("receiveNo", ReceiveNo);
            List<ItemNameList> list = _IDapperService.GetAllBySP<ItemNameList>(SP, p).ToList();

        
            return list;
        }

        public int Add(ItemTransfer entity)
        {
            throw new NotImplementedException();
        }
    }
}
