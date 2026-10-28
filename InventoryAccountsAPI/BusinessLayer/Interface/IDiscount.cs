using FTSoftAPI.Service.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IDiscount :ICommonMethod<Discount>
    {
        List<RptDiscount> GetDiscountList(int DiscountId);
        List<RptItem> GetAllItemListWithStock(int WhId);
        List<RptItem> GetAllItemListWithStockForEdit(int WhId);
        List<RptItem> GetAllDiscountedItemByDiscountId(int DiscountId);

        // List<DiscountDetails> SaveDiscountDetails(DiscountWithDetailsRequest request);
        Task<(bool Success, string Message)> SaveDiscountDetailsAsync(DiscountWithDetailsRequest request);
        Task<(bool Success, string Message)> UpdateDiscountDetailsAsync(Discount request);
        public int AddCardAndVoucher(GiftVoucher entity);
        List<RptDiscount> GetGiftCardAndVoucherList(int WhId);
        List<RptGiftCardAndVoucher> GetGiftCardAndVoucherDetailsList(int Id);
        public int UpdateCardAndVoucher(RptGiftCardAndVoucher entity);
        List<RptGiftCardAndVoucher> GetAmountByCVName(string name);


    }
}
