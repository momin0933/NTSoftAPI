using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.Common;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ItemReceiveManager : IItemReceive
    {
        //readonly FTDbContext _dbContext;
        //readonly NTSoftDbContextFactory _dbContext;
        //private readonly IDapperService _IDapperService;
        //ICommonService _ICommonService;
        private readonly NTSoftDbContextFactory _factory;
        private readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _IDapperService;
        private readonly ICommonService _ICommonService;
        //public ItemReceiveManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);          
        //}
        public ItemReceiveManager(NTSoftDbContextFactory factory,IDapperService dapperService,ILogger<ItemManager> logger)
        {
            _factory = factory;
            _dbContext = _factory.CreateDbContext();

            _IDapperService = dapperService;
            _ICommonService = new CommonService(_factory);
        }
        public List<ItemReceive> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<ItemReceive>().Where(x=>x.IsActive == true).ToList();
            }
            catch
            {
                throw;
            }
        }        
        public int Add(ItemReceive entity)
        {
            try
            {
                //var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).FirstOrDefault();
                //if(stock != null)
                //{
                //    stock.ReceiveQty = stock.ReceiveQty == null ? entity.TotalQty : stock.ReceiveQty+ entity.TotalQty;
                //    stock.StockQty = stock.StockQty == null ? entity.TotalQty : stock.StockQty + entity.TotalQty;
                //}
                //_ICommonService.Add<ItemReceive>(entity);
                //_ICommonService.Update<ItemStock>(stock);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Update(ItemReceive entity)
        {
            try
            {
                //_ICommonService.Update<ItemReceive>(entity);
                //var rcvQty = _ICommonService.GetAll<ItemReceive>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).Sum(c => (int?)c.TotalQty) ?? 0;
                //var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).FirstOrDefault();
                //if (stock != null)
                //{
                //    stock.ReceiveQty = rcvQty;
                //    stock.StockQty = stock.IssueQty == null ? rcvQty : rcvQty + stock.IssueQty;
                //}
                
                //_ICommonService.Update<ItemStock>(stock);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Remove(ItemReceive entity)
        {
            try
            {
                //_ICommonService.RemoveById<ItemReceive>(entity.Id);
                //var rcvQty = _ICommonService.GetAll<ItemReceive>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).Sum(c => (int?)c.TotalQty) ?? 0;
                //var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).FirstOrDefault();
                //if (stock != null)
                //{
                //    stock.ReceiveQty = rcvQty;
                //    stock.StockQty = stock.IssueQty == null ? rcvQty : rcvQty + stock.IssueQty;
                //}

                //_ICommonService.Update<ItemStock>(stock);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public int AddAndUpdate(ItemReceive entity, string paymentType)
        {
            try
            {
                foreach (var dt in entity.ItemRcvDetailsList)
                {
                    var stock = _ICommonService.GetAll<ItemStock>().FirstOrDefault(x => x.ItemId == dt.ItemId && x.WhId == entity.WhId && x.VariantId==dt.VariantId);
                    if (stock != null)
                    {
                        stock.ReceiveQty = stock.ReceiveQty.GetValueOrDefault() + dt.Qty;
                        stock.StockQty = stock.StockQty.GetValueOrDefault() + dt.Qty;                       
                        _ICommonService.Update<ItemStock>(stock);
                    }

                    //var item = _ICommonService.GetAll<ItemVariant>().FirstOrDefault(a => a.Id == dt.ItemId);
                    //if (item != null)
                    //{
                    //    item.CostingPrice = dt.CostingPrice;
                        
                    //    item.SellingPrice = dt.SellingPrice;
                    //    _ICommonService.Update<ItemVariant>(item);
                    //}

                    var sqlQuery = @"INSERT INTO POStblItemTransaction 
                    (ItemId,VariantId ,WhId, TranDt, Qty, Status, Remarks, EntryDate, EntryBy, IsActive)
                     VALUES (" + dt.ItemId + ","+ dt.VariantId +",  " + entity.WhId + ", GETDATE(), " + dt.Qty + ", 'Direct-Received', '" + entity.Remarks + "', GETDATE(), '" + entity.EntryBy + "', 1);";

                    _dbContext.Database.ExecuteSqlRaw(sqlQuery);
                }

                //entity.Status = "Direct-Received";
                _ICommonService.Add<ItemReceive>(entity);
                //return entity.Id;
                int itemReceiveId = entity.Id;
                if (!string.IsNullOrEmpty(paymentType) &&
                     paymentType.ToLower() == "cash")
                {
                    decimal totalAmount = entity.ItemRcvDetailsList.Sum(x =>
                        (x.Qty ?? 0) * (x.CostingPrice ?? 0));

                    var supplierPayment = new POStblSupplierPayment
                    {
                        SupplierId = entity.SupplierId,
                        PaymentDate = DateTime.Now,
                        TotalPaid = totalAmount,
                        Remarks = "cash Purchase",
                        EntryDate = DateTime.Now,
                        EntryBy = entity.EntryBy,
                        IsActive = true
                    };

                    _ICommonService.Add<POStblSupplierPayment>(supplierPayment);

                    var supplierPaymentDetails = new POStblSupplierPaymentDetails
                    {
                        PaymentId = supplierPayment.Id,
                        InvoiceId = itemReceiveId,
                        PaidAmount = totalAmount,
                        Remarks = "cash Purcase",
                        EntryDate = DateTime.Now,
                        EntryBy = entity.EntryBy,
                        IsActive = true
                    };

                    _ICommonService.Add<POStblSupplierPaymentDetails>(supplierPaymentDetails);
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception("Error occurred while adding/updating items.", ex);
            }
        }




        //public int AddAndUpdate(ItemReceive entity)
        //{
        //    try
        //    {
        //        foreach (var dt in entity.ItemRcvDetailsList)
        //        {
        //            var stock = _ICommonService.GetAll<ItemStock>().FirstOrDefault(x => x.ItemId == dt.ItemId && x.WhId == entity.WhId);
        //            if (stock != null)
        //            {
        //                stock.ReceiveQty = stock.ReceiveQty.GetValueOrDefault() + dt.Qty;
        //                stock.StockQty = stock.StockQty.GetValueOrDefault() + dt.Qty;
        //                _ICommonService.Update<ItemStock>(stock);
        //            }

        //            var item = _ICommonService.GetAll<Item>().FirstOrDefault(a => a.Id == dt.ItemId);
        //            if (item != null)
        //            {
        //                item.CostingPrice = dt.CostingPrice;
        //                item.SellingPrice = dt.SellingPrice;
        //                _ICommonService.Update<Item>(item);
        //            }
        //        }

        //        _ICommonService.Add<ItemReceive>(entity);
        //        return entity.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception here
        //        throw new Exception("Error occurred while adding/updating items.", ex);
        //    }
        //}

        //public List<ItemRecievewWithItemDetails> GetItemRecieveWithItemDetails(int wareHouse)
        //{
        //    string SP = "SP_ItemReceiveWithItemDtails";
        //    DynamicParameters p = new DynamicParameters();
        //    if (wareHouse == 0)
        //    {
        //        p.Add("@WhId", wareHouse);
        //    }
        //    else
        //    {

        //        p.Add("@WhId", wareHouse);
        //    }
           

        //    List<ItemRecievewWithItemDetails> ItemList = _IDapperService.GetAllBySP<ItemRecievewWithItemDetails>(SP, p).ToList();
        //    return ItemList;
        //}

        public int UpdateForEdit(ItemReceivePerams entity)
        {
            try
            {
                int itemId = 0;/*_ICommonService.GetAll<ItemReceive>().Where(x => x.Id == entity.Id).FirstOrDefault().ItemId;*/
                string supplierName = _ICommonService.GetAll<ItemReceive>().Where(x => x.SupplierId == entity.SuplierId).FirstOrDefault().Supplier;

                ItemReceive itemReceive = new ItemReceive();

                itemReceive.Id = entity.Id;
                itemReceive.WhId = entity.WhId;
                itemReceive.ReceiveDt = entity.ReceiveDt;
                itemReceive.SupplierId = entity.SuplierId;
                itemReceive.Supplier = supplierName;
                itemReceive.TotalQty = entity.Qty;
                itemReceive.UpdateBy = entity.UpdateBy;
                itemReceive.UpdateDate = entity.UpdateDate;


                var itemTable = _ICommonService.GetAll<Item>().Where(x => x.Id == itemId).FirstOrDefault();
                if(itemTable!= null)
                {
                    //itemTable.CostingPrice = entity.CostingPrice;
                   /// itemTable.SellingPrice = entity.ItemSellingPrice;
                    itemTable.Vat = entity.ItemVat;
                    itemTable.Discount = entity.ItemDiscount;
                };
                var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == itemId).FirstOrDefault();
                if (stock != null)// need to check again
                {
                    stock.ReceiveQty = stock.ReceiveQty == null ? entity.Qty : stock.ReceiveQty + entity.Qty;
                    stock.StockQty = stock.StockQty == null ? entity.Qty : stock.StockQty + entity.Qty;
                }
                _ICommonService.Update<ItemReceive>(itemReceive);
                _ICommonService.Update<Item>(itemTable);
                _ICommonService.Update<ItemStock>(stock);

                return entity.Id;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        
        public List<RtpReceiveItemList> GetAllReceivedItemsByRcvId(int rcvId)
        {
            try
            {
                string SP = "SP_ReceivedItemListDetails";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@RcvId", rcvId);
                var list = _IDapperService.GetAllBySP<RtpReceiveItemList>(SP,p).ToList();
                return list;
            }
            catch{
                throw;
            }
          
        }

        //public int SellingPriceUpdate(PriceUpdateRequest entity)
        //{
        //    try
        //    {
        //        PriceLog log = new PriceLog()
        //        {
        //            ReceiveNo= entity.ReceiveNo,
        //            EntryBy= entity.EntryBy,
        //            EntryDate=DateTime.Now
        //        };
        //        _ICommonService.Add<PriceLog>(log);
        //        foreach (var dt in entity.Items)
        //        {
                    

        //            var sqlQuery = @"Update POStblItemVariant set SellingPrice="+ dt.SellingPrice+" where ItemId="+ dt.ItemId+"and id="+dt.Id+"";

        //            _dbContext.Database.ExecuteSqlRaw(sqlQuery);

        //            PriceLogDetails details = new PriceLogDetails()
        //            {
        //                LogId=log.Id,
        //                ItemId=dt.ItemId,
        //                VariantId=dt.Id,
        //                OldPrice=dt.CostingPrice,
        //                NewPrice=dt.SellingPrice,
        //                EntryBy=entity.EntryBy,
        //                EntryDate=DateTime.Now
        //            };
        //            _ICommonService.Add<PriceLogDetails>(details);
        //        }
        //        return entity.Items.Count;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception here
        //        throw new Exception("Error occurred while adding/updating items.", ex);
        //    }
        //}



        public List<string> LastVoucherNoWithPrefix(string typeString)
        {
            try
            {
                string query = $@"
            SELECT TOP 1 ReceiveNo
            FROM POStblItemRcv
            WHERE ReceiveNo LIKE '{typeString}%'
            ORDER BY CAST(
                SUBSTRING(ReceiveNo, LEN('{typeString}') + 1, LEN(ReceiveNo))
                AS INT
            ) DESC";

                return _IDapperService
                    .GetAllByQuery<string>(query)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public int AddAndUpdateNew(ItemReceive entity, string paymentType, int? paymentLedgerId, string voucherNumber)
        {
            try
            {
                foreach (var dt in entity.ItemRcvDetailsList)
                {
                    var stock = _ICommonService.GetAll<ItemStock>().FirstOrDefault(x => x.ItemId == dt.ItemId && x.WhId == entity.WhId && x.VariantId == dt.VariantId);
                    if (stock != null)
                    {
                        stock.ReceiveQty = stock.ReceiveQty.GetValueOrDefault() + dt.Qty;
                        stock.StockQty = stock.StockQty.GetValueOrDefault() + dt.Qty;
                        _ICommonService.Update<ItemStock>(stock);
                    }

                    //var item = _ICommonService.GetAll<ItemVariant>().FirstOrDefault(a => a.Id == dt.ItemId);
                    //if (item != null)
                    //{
                    //    item.CostingPrice = dt.CostingPrice;

                    //    item.SellingPrice = dt.SellingPrice;
                    //    _ICommonService.Update<ItemVariant>(item);
                    //}

                    var sqlQuery = @"INSERT INTO POStblItemTransaction 
                    (ItemId,VariantId ,WhId, TranDt, Qty, Status, Remarks, EntryDate, EntryBy, IsActive)
                     VALUES (" + dt.ItemId + "," + dt.VariantId + ",  " + entity.WhId + ", GETDATE(), " + dt.Qty + ", 'Direct-Received', '" + entity.Remarks + "', GETDATE(), '" + entity.EntryBy + "', 1);";

                    _dbContext.Database.ExecuteSqlRaw(sqlQuery);
                }

                //entity.Status = "Direct-Received";
                _ICommonService.Add<ItemReceive>(entity);
                //return entity.Id;
                int itemReceiveId = entity.Id;
                if (!string.IsNullOrEmpty(paymentType) && paymentType.ToLower() != "credit")
                {
                    decimal totalAmount =
                        entity.ItemRcvDetailsList.Sum(x =>
                            (x.Qty ?? 0) *
                            (x.CostingPrice ?? 0)
                        );

                    var supplierPayment =
                        new POStblSupplierPayment
                        {
                            SupplierId = entity.SupplierId,

                            PaymentDate = DateTime.Now,

                            TotalPaid = totalAmount,

                            VoucherNumber = voucherNumber,

                            Remarks = paymentType + " Purchase",

                            EntryDate = DateTime.Now,

                            EntryBy = entity.EntryBy,

                            IsActive = true
                        };

                    _ICommonService.Add<POStblSupplierPayment>(
                        supplierPayment
                    );

                    var supplierPaymentDetails =
                        new POStblSupplierPaymentDetails
                        {
                            PaymentId = supplierPayment.Id,

                            InvoiceId = itemReceiveId,

                            PaidAmount = totalAmount,

                            Remarks = paymentType + " Purchase",

                            EntryDate = DateTime.Now,

                            EntryBy = entity.EntryBy,

                            IsActive = true
                        };

                    _ICommonService.Add<POStblSupplierPaymentDetails>(
                        supplierPaymentDetails
                    );
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception("Error occurred while adding/updating items.", ex);
            }
        }
    }
}
