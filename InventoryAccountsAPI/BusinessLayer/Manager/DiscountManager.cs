using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using Microsoft.EntityFrameworkCore;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class DiscountManager : IDiscount
    {
        readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //public DiscountManager(NTSoftDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public DiscountManager(NTSoftDbContext dbContext,DapperService dapperService, NTSoftDbContextFactory dbContextFactory)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(dbContextFactory);
        }
        public int Add(Discount entity)
        {
            _ICommonService.Add<Discount>(entity);
            return entity.Id;
        }

      

        public List<Discount> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<RptItem> GetAllDiscountedItemByDiscountId(int DiscountId)
        {
            string SP = "Sp_posDiscount";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 3);

             p.Add("@DiscountId", DiscountId);

            List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).ToList();
            return ItemList;
        }

        public List<RptItem> GetAllItemListWithStock(int WhId)
        {
            string SP = "Sp_posDiscount";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 2);

             p.Add("@WhId", WhId);

            List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x => x.EntryDate).ToList();
            return ItemList;
        }

        public List<RptDiscount> GetDiscountList(int DiscountId)
        {
            try
            {
                if(DiscountId > 0)
                {
                    string SP = "Sp_posDiscount";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 4);

                    p.Add("@DiscountId", DiscountId);

                    List<RptDiscount> ItemList = _IDapperService.GetAllBySP<RptDiscount>(SP, p).ToList();
                    return ItemList;
                }
                else
                {
                    string SP = "Sp_posDiscount";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 1);

                    List<RptDiscount> ItemList = _IDapperService.GetAllBySP<RptDiscount>(SP, p).OrderByDescending(x => x.Id).ToList();
                    return ItemList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<RptItem> GetAllItemListWithStockForEdit(int WhId)
        {
            string SP = "Sp_posDiscount";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 5);

            p.Add("@WhId", WhId);

            List<RptItem> ItemList = _IDapperService.GetAllBySP<RptItem>(SP, p).OrderByDescending(x => x.EntryDate).ToList();
            return ItemList;
        }

        public int Remove(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Message)> SaveDiscountDetailsAsync(DiscountWithDetailsRequest request)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var discount in request.Discounts)
                    {
                        _ICommonService.Add(discount);
                    }
                    await _dbContext.SaveChangesAsync();

                   
                    await transaction.CommitAsync();

                    return (true, "Data saved successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"An error occurred while saving data: {ex.Message}");
                }
            }
        }

        public int Update(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Message)> UpdateDiscountDetailsAsync(Discount discount)
        {


            try
            {
                var discountDetils = discount.DiscountDetails.ToList();

                Discount discounts = new Discount
                {
                    Id=discount.Id,
                   Name=discount.Name,
                   Type=discount.Type,
                   Value=discount.Value,
                   EndDate=discount.EndDate,
                   StartDate=discount.StartDate,
                   UpdateBy=discount.UpdateBy,
                   UpdateDate=DateTime.Now
                };

                _ICommonService.Update<Discount>(discounts);


                var existingDetails = await _dbContext.POStblDiscountDetails
                   .Where(d => d.DiscountId == discount.Id)
                   .ToListAsync();

                foreach (var existingDetail in existingDetails)
                {
                    existingDetail.IsActive = false; // Set IsActive to false for all existing details

                    _ICommonService.Update<DiscountDetails>(existingDetail);
                }


                foreach (var item in discountDetils)
                {

                   DiscountDetails discountDetails = new DiscountDetails
                   {

                      VariantId= item.VariantId,
                      DiscountId=item.DiscountId,
                      WhId=item.WhId,
                      UpdateBy=item.UpdateBy,
                      UpdateDate=DateTime.Now
                   };
                   _ICommonService.Add<DiscountDetails>(discountDetails);

                 };

               
                return (true, "Data saved successfully.");
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                return (false, $"An error occurred while saving data: {ex.Message}");
            }
        }

        public int AddCardAndVoucher(GiftVoucher entity)
        {
            _ICommonService.Add(entity);
            return entity.Id;
        }

        public List<RptDiscount> GetGiftCardAndVoucherList(int WhId)
        {
            if(WhId != 0)
            {
                string SP = "Sp_posDiscount";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 6);
                p.Add("@WhId", WhId);
                List<RptDiscount> ItemList = _IDapperService.GetAllBySP<RptDiscount>(SP, p).ToList();
                return ItemList;
            }
            else
            {
                string SP = "Sp_posDiscount";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 7);
                List<RptDiscount> ItemList = _IDapperService.GetAllBySP<RptDiscount>(SP, p).ToList();
                return ItemList;
            }
            
        }

        public List<RptGiftCardAndVoucher> GetGiftCardAndVoucherDetailsList(int Id)
        {
            string SP = "Sp_posDiscount";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 8);
            p.Add("@GiftVoucherId", Id);
            List<RptGiftCardAndVoucher> ItemList = _IDapperService.GetAllBySP<RptGiftCardAndVoucher>(SP, p).ToList();
            return ItemList;
        }

        public int UpdateCardAndVoucher(RptGiftCardAndVoucher entity)
        {
            GiftVoucher giftVoucher = new GiftVoucher
            {
               Id= (int)entity.Id,
               Name=entity.Name,
               Type=entity.Type,
               Amount= (int)entity.Amount,
               Qty= (int)entity.Qty,
               StartDate= (DateTime)entity.StartDate,
               EndDate= (DateTime)entity.EndDate,
               WhId= (int)entity.WhId,
               UpdateBy=entity.UpdateBy,
               UpdateDate=entity.UpdateDate
            };
            _ICommonService.Update<GiftVoucher>(giftVoucher);
            return giftVoucher.Id;
        }

        public List<RptGiftCardAndVoucher> GetAmountByCVName(string name)
        {
            string SP = "Sp_posDiscount";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 9);
            p.Add("@name", name);
            List<RptGiftCardAndVoucher> ItemList = _IDapperService.GetAllBySP<RptGiftCardAndVoucher>(SP, p).ToList();
            return ItemList;
        }
    }
}
