using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ItemManager : IItem
    {
        private readonly ILogger<ItemManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;

        public ItemManager(NTSoftDbContextFactory dbContext, IDapperService dapperService, ILogger<ItemManager> logger)
        {
            
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }    

        public List<Item> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<Item>().ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<RptItem> GetAllItemListWithStock(int WHId, string type)
        {
            try
            {
                string SP = "SP_ItemStock";
                DynamicParameters p = new DynamicParameters();
                if (type == "Details")
                {
                    p.Add("@QueryChecker", 1);
                }
                else
                {
                    p.Add("@QueryChecker", 2);

                }
                p.Add("@WHId", WHId);

                List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x=>x.EntryDate).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<RptItem> GetAllItemListWithStockByStyleNo(string styleNo, int WhId)
        {
            try
            {
                string SP = "SP_ItemStock";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);

                p.Add("@styleNo", styleNo);
                p.Add("@WhId", WhId);

                List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x => x.EntryDate).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       

        public int Add(Item entity)
        {
            try
            {
                var existingItem = _ICommonService.GetAll<Item>()
                    .FirstOrDefault(i => i.StyleNo == entity.StyleNo
                                      && i.Brand == entity.Brand
                                      && i.Category == entity.Category
                                      && i.ProductType == entity.ProductType
                                      && i.Description == entity.Description);

                if (existingItem != null)
                {
                    throw new Exception("A product with the same StyleNo, Brand, Category, and ProductType already exists.");
                }

                // --- Create main Item object ---
                var itemPost = new Item
                {
                    Brand = entity.Brand,
                    Category = entity.Category,
                    Description = entity.Description,
                    StyleNo = entity.StyleNo,
                    ProductType = entity.ProductType,
                    ImgPath = entity.ImgPath,
                    IsStockAlertQty = entity.IsStockAlertQty,
                    EntryBy = entity.EntryBy,
                    EntryDate = entity.EntryDate,
                    Status = "New",
                    IsWebsite = false,
                    ItemsList = new List<ItemVariant>(),
                    ItemStockList = new List<ItemStock>()
                };

                // --- Add child entities before saving ---
                if (entity.ItemsList != null && entity.ItemsList.Any())
                {
                    foreach (var variant in entity.ItemsList)
                    {
                        var variantPost = new ItemVariant
                        {
                            Color = variant.Color,
                            Size = variant.Size,
                            SKU = variant.SKU,
                            Barcode = variant.Barcode,
                            SellingPrice = variant.SellingPrice,
                            CostingPrice = variant.CostingPrice,
                            Unit = variant.Unit,
                            Vat=variant.Vat,
                            VatCalculationType = variant.VatCalculationType,
                            VatType =variant.VatType,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            ItemStockList = new List<ItemStock>()
                        };

                        itemPost.ItemsList.Add(variantPost);

                        if (entity.ItemStockList != null && entity.ItemStockList.Any())
                        {
                            foreach (var stock in entity.ItemStockList)
                            {
                                var stockPost = new ItemStock
                                {
                                    WhId = stock.WhId,
                                    StockQty = stock.StockQty,
                                    ReceiveQty = stock.ReceiveQty,
                                    IssueQty = stock.IssueQty,
                                    ReturnQty = stock.ReturnQty,
                                    AdjQty = stock.AdjQty,
                                    PrevQty = stock.PrevQty,
                                    EntryBy = entity.EntryBy,
                                    EntryDate = entity.EntryDate,
                                    ItemVariant = variantPost
                                };

                                itemPost.ItemStockList.Add(stockPost);
                            }
                        }
                    }
                }

                // --- Add the item (and its children) only once ---
                _ICommonService.Add<Item>(itemPost);

                // ⚡ Now save everything once — EF will handle all inserts transactionally
                _ICommonService.Save();  // Make sure this exists in your service

                return itemPost.Id;
            }
            catch
            {
                throw;
            }
        }

        public int Update(Item entity)
        {
            try
            {
                _ICommonService.Update<Item>(entity);              
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }
        public int Remove(Item entity)
        {
            try
            {
                //_ICommonService.RemoveById<Item>(entity.Id);
                //var rcvQty = _ICommonService.GetAll<Item>().Where(x => x.ItemId == entity.ItemId && x.WhId == entity.WhId).Sum(c => (int?)c.Qty) ?? 0;
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

        public int DeleteById(int id) /// Getting Error when want to delete data 
        {
            try
            {
                
                Item itemForDataBind = _ICommonService.Get<Item>(id);
                //int stockItemId = itemForDataBind.ItemStockList.Select(x => x.ItemId).FirstOrDefault();
                //ItemStock stock = _ICommonService.Get<ItemStock>(stockItemId);

                if (itemForDataBind != null)
                {
                    //_ICommonService.RemoveById<Item>(id);
                    //if (stockItemId != null)
                    //{
                    //    _ICommonService.RemoveById<ItemStock>(stockItemId);
                    //}
                    return id;
                }

                    //List<ItemStock> itemStockList = new List<ItemStock>();
                    //if (itemForDataBind.ItemStockList != null)
                    //{
                    //    foreach (var stockData in itemForDataBind.ItemStockList)
                    //    {
                    //        ItemStock itemStock = new ItemStock();
                    //        itemStock.Id = stockData.Id;
                    //        itemStock.ItemId = stockData.ItemId;
                    //        itemStock.WhId = stockData.WhId;
                    //        itemStock.IssueQty = stockData.IssueQty;
                    //        itemStock.ReceiveQty = stockData.ReceiveQty;
                    //        itemStock.StockQty = stockData.StockQty;
                    //        itemStock.UpdateBy = "Admin";
                    //        itemStock.UpdateDate = DateTime.Now;
                    //        itemStock.IsActive = false;
                    //        itemStockList.Add(itemStock);
                    //    }
                    //}


                    //Item itemForStatusUpdate = new Item
                    //{
                    //    Name = itemForDataBind.Name,
                    //    Code = itemForDataBind.Color,
                    //    BarCode = itemForDataBind.BarCode,
                    //    SKU = itemForDataBind.SKU,
                    //    Color = itemForDataBind.Color,
                    //    Size = itemForDataBind.Size,
                    //    Brand = itemForDataBind.Brand,
                    //    Category = itemForDataBind.Category,
                    //    Description = itemForDataBind.Description,
                    //    CostingPrice = itemForDataBind.CostingPrice,
                    //    SellingPrice = itemForDataBind.SellingPrice,
                    //    Vat = itemForDataBind.SellingPrice,
                    //    Discount = itemForDataBind.Discount,
                    //    IsActive = false,
                    //    UpdateDate = DateTime.Now,
                    //    UpdateBy = "Admin",
                    //    //ItemStockList = itemStockList

                    //    //foreach ( var stockItem in itemForStatusUpdate.ItemStockList)
                    //    //{
                    //    //    stockItem.ItemId = id;
                    //    //}
                    //};
                    //_ICommonService.RemoveById<ItemStock>()        
                    //    _ICommonService.Remove<Item>(itemForStatusUpdate);
              
                else
                {
                    return id = 0;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string CountItem()
        {
            var lastItem = _ICommonService.GetAll<ItemVariant>()
                    .OrderByDescending(i => i.Id)  // Ordering by ItemId (change as necessary)
                    .FirstOrDefault();  // Get the last item

            // Check if an item exists
            if (lastItem != null && !string.IsNullOrEmpty(lastItem.Barcode))
            {
                // Trim the last 4 digits of the BarCode
                string trimmedBarCode = lastItem.Barcode.Length >= 4
                             ? lastItem.Barcode.Substring(lastItem.Barcode.Length - 4)  // Extract last 4 digits
                             : lastItem.Barcode;

                return trimmedBarCode;
            }

            return string.Empty;
        }
        public string GetItemNameById(int id)
        {
            try
            {
                string itemName = _ICommonService.GetAll<Item>().Where(x=>x.Id == id).FirstOrDefault().StyleNo;
                return itemName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public VariantListForDropdown? GetItemDetailsById(int id)
        //{
        //    try 
        //    {

        //       var itemNameList = _IDapperService.GetAllByQuery<VariantListForDropdown>("select v.Id,StyleNo,Barcode,Color,Size from POStblItemVariant v Join POStblItem i on v.ItemId=i.Id where v.Id = '" + id + "' and v.isActive = 1").OrderByDescending(x => x.Id).FirstOrDefault();
        //        return itemNameList;

        //    }
        //    catch 
        //    {
        //        throw;
        //    }
           
        //}

        //public List<RptSupplierWiseReport> GetSupplierWiseData(DateTime fromDate, DateTime toDate)
        //{
        //    try
        //    {
        //        string SP = "Sp_SupplierWiseReport";
        //        DynamicParameters p = new DynamicParameters();
        //        p.Add("fromDate", fromDate);
        //        p.Add("@toDate", toDate);
        //        p.Add("@QueryChecker", 1);

        //        List<RptSupplierWiseReport> ItemList = _IDapperService.GetAllBySP<RptSupplierWiseReport>(SP, p).ToList();
        //        return ItemList;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public List<RptItem> GetAllVariantList(int WHId)
        {
            try
            {
                string SP = "SP_ItemStock";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);

                p.Add("@WHId", WHId);

                List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x => x.EntryDate).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public int UpdateItemVariant(ItemVariantUpdate itemVariant)
        //{
        //    if(itemVariant.ItemId != 0)
        //    {
        //        //var query = "UPDATE PostblItem SET StyleNo='" + itemVariant.StyleNo + "', Description = '" + itemVariant.Description + "',Category='"+itemVariant.Category+"' ,UpdateBy='" + itemVariant.UpdateBy + "' , UpdateDate='" + itemVariant.UpdateDate + "' WHERE Id='" + itemVariant.ItemId + "'";
        //        var query =
        //            "UPDATE POStblItem SET " +
        //            "StyleNo='" + (itemVariant.StyleNo ?? "") + "', " +
        //            "Brand='" + (itemVariant.Brand ?? "") + "', " +
        //            "Category='" + (itemVariant.Category ?? "") + "', " +
        //            "ProductType='" + (itemVariant.ProductType ?? "") + "', " +
        //            "Description='" + (itemVariant.Description ?? "") + "', " +
        //            //"Vat=" + (itemVariant.Vat ?? 0) + ", " +
        //            //"Discount=" + (itemVariant.Discount ?? 0) + ", " +
        //            //"ImgPath='" + (itemVariant.ImgPath ?? "") + "', " +
        //            //"PMTitle='" + (itemVariant.PMTitle ?? "") + "', " +
        //            //"PMDescription='" + (itemVariant.PMDescription ?? "") + "', " +
        //            //"IsStockAlertQty=" + (itemVariant.IsStockAlertQty ?? 0) + ", " +
        //            //"EcomThumbnailUrls='" + (itemVariant.EcomThumbnailUrls ?? "") + "', " +
        //            //"EcomPreviewUrls='" + (itemVariant.EcomPreviewUrls ?? "") + "', " +
        //            //"EcomDescription='" + (itemVariant.EcomDescription ?? "") + "', " +
        //            //"EcomSpecification='" + (itemVariant.EcomSpecification ?? "") + "', " +
        //            //"Remarks='" + (itemVariant.Remarks ?? "") + "', " +
        //            "UpdateDate='" + itemVariant.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
        //            "UpdateBy='" + (itemVariant.UpdateBy ?? "") + "' " +
        //            //"IsWebsite=" + (itemVariant.IsWebsite == true ? 1 : 0) + ", " +
        //            //"IsActive=" + (itemVariant.IsActive == true ? 1 : 0) + ", " +
        //            //"Status='" + (itemVariant.Status ?? "") + "' " +
        //            "WHERE Id=" + itemVariant.ItemId;
        //        _IDapperService.UpdateByquery(query);
        //    }
        //    if(itemVariant.Id != 0)
        //    {
        //        //var query = "UPDATE PostblItemVariant SET Color='" + itemVariant.Color + "', Size = '" + itemVariant.Size + "',Remarks='"+itemVariant.Remarks+"' ,UpdateBy='" + itemVariant.UpdateBy + "' , UpdateDate='" + itemVariant.UpdateDate + "' WHERE Id='" + itemVariant.Id + "'";
        //            var query = "UPDATE PostblItemVariant SET " +
        //            "Color = '" + itemVariant.Color + "', " +
        //            "Size = '" + itemVariant.Size + "', " +
        //            "Remarks = '" + itemVariant.Remarks + "', " +
        //            "CostingPrice = " + (itemVariant.CostingPrice ?? 0) + ", " +
        //            "SellingPrice = " + (itemVariant.SellingPrice ?? 0) + ", " +
        //            "Unit = '" + itemVariant.Unit + "', " +
        //            "VatCalculationType = '" + itemVariant.VatCalculationType + "', " +
        //            "Vat = " + (itemVariant.Vat ?? 0) + ", " +
        //            "VatType = '" + itemVariant.VatType + "', " +
        //            "UpdateBy = '" + itemVariant.UpdateBy + "', " +
        //            "UpdateDate = '" + itemVariant.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
        //    "WHERE Id = " + itemVariant.Id;

        //        _IDapperService.UpdateByquery(query);

        //    }
        //    return itemVariant.Id;
        //}

        public int AddVariantToExistingItem(List<ItemVariant> variants)
        {
            if (variants == null || !variants.Any())
                throw new ArgumentException("No variants provided.");
            var grouped = variants.GroupBy(v => v.ItemId);

            foreach (var group in grouped)
            {
                int itemId = group.Key ?? 0;
                if (itemId <= 0) continue;

                var item = _ICommonService.GetAll<Item>()
                    .FirstOrDefault(i => i.Id == itemId);

                if (item == null)
                    throw new Exception($"Item {itemId} not found.");

                item.ItemsList ??= new List<ItemVariant>();

                foreach (var entity in group)
                {
                    var variantPost = new ItemVariant
                    {
                        ItemId = item.Id,
                        Color = entity.Color,
                        Size = entity.Size,
                        SKU = entity.SKU,
                        Barcode = entity.Barcode,
                        CostingPrice = entity.CostingPrice,
                        SellingPrice = entity.SellingPrice,
                        Unit = entity.Unit,
                        Vat = entity.Vat,
                        VatType = entity.VatType,
                        VatCalculationType = entity.VatCalculationType,
                        EntryBy = entity.EntryBy,
                        EntryDate = entity.EntryDate,
                        ItemStockList = new List<ItemStock>()
                    };

                    if (entity.ItemStockList != null)
                    {
                        foreach (var stock in entity.ItemStockList)
                        {
                            variantPost.ItemStockList.Add(new ItemStock
                            {
                                ItemId = item.Id,
                                ItemVariant = variantPost,
                                WhId = stock.WhId,
                                StockQty = 0,
                                ReceiveQty = 0,
                                IssueQty = 0,
                                ReturnQty = 0,
                                AdjQty = 0,
                                PrevQty = 0,
                                EntryBy = entity.EntryBy,
                                EntryDate = entity.EntryDate
                            });
                        }
                    }

                    item.ItemsList.Add(variantPost);
                }


                _ICommonService.Update(item);
            }


            _ICommonService.Save();

            return variants.Count;
        }

       

        public int UpdateItemImage(int itemId, string imgPath, string updateBy)
        {
            var item = _ICommonService.GetAll<Item>()
                        .FirstOrDefault(x => x.Id == itemId);

            if (item == null)
                throw new Exception("Item not found");

            item.ImgPath = imgPath;
            item.UpdateDate = DateTime.Now;
            item.UpdateBy = updateBy;   

            _ICommonService.Update(item);
            _ICommonService.Save();

            return item.Id;
        }
    }
}
