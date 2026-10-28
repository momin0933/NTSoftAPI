using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscount _Service;

        public DiscountController(IDiscount item)
        {
            _Service = item;
        }
        [Route("GetDiscountList")]
        [HttpGet]
        public List<RptDiscount> GetDiscountList(int DiscountId)
        {
            try
            {
                var ItemList = _Service.GetDiscountList(DiscountId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }
    
        [Route("GetAllItemWithStock")]
        [HttpGet]
        public List<RptItem> GetAllItemWithStock(int WhId)
        {
            try
            {
                var ItemList = _Service.GetAllItemListWithStock(WhId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }

        //[Route("SaveDiscountDetails")]
        //[HttpPost]
        //public IActionResult SaveDiscountDetails(List<DiscountDetails> discount )
        //{
        //    _Service.SaveDiscountDetails(discount);
        //    return Ok(discount);
        //}

        [Route("SaveDiscountDetails")]
        [HttpPost]
        public async Task<IActionResult> AddDiscountWithDetails([FromBody] DiscountWithDetailsRequest request)
        {


            var result = await _Service.SaveDiscountDetailsAsync(request);

            if (result.Success)
            {
                return Ok(new { success = true, message = "Data saved successfully." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving data." });
            }
        }

        [Route("DiscountedItemList")]
        [HttpGet]
        public List<RptItem> GetAllDiscountedItemByDiscountId(int DiscountId)
        {
            try
            {
                var ItemList = _Service.GetAllDiscountedItemByDiscountId(DiscountId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }
        [Route("GetAllItemWithStockForEdit")]
        [HttpGet]
        public List<RptItem> GetAllItemWithStockForEdit(int WhId)
        {
            try
            {
                var ItemList = _Service.GetAllItemListWithStockForEdit(WhId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }
        [Route("UpdateDiscountDetails")]
        [HttpPost]
        public async Task<IActionResult> UpdateDiscountWithDetails([FromBody] Discount request)
        {


            var result = await _Service.UpdateDiscountDetailsAsync(request);

            if (result.Success)
            {
                return Ok(new { success = true, message = "Data saved successfully." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving data." });
            }
        }

        [Route("AddCardAndVoucher")]
        [HttpPost]
        public  int AddCardAndVoucher([FromBody] GiftVoucher giftVoucher)
        {


            var result =  _Service.AddCardAndVoucher(giftVoucher);

            return result;
        }

        [Route("GetGiftCardAndVoucherList")]
        [HttpGet]
        public List<RptDiscount> GetGiftCardAndVoucherList(int whId)
        {
            try
            {
                var ItemList = _Service.GetGiftCardAndVoucherList(whId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetGiftCardAndVoucherDetailsList")]
        [HttpGet]
        public List<RptGiftCardAndVoucher> GetGiftCardAndVoucherDetailsList(int Id)
        {
            try
            {
                var ItemList = _Service.GetGiftCardAndVoucherDetailsList(Id);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("UpdatecardandVoucher")]
        [HttpPost]
        public int UpdateCardAndVoucher(RptGiftCardAndVoucher giftVoucher)
        {


            var result = _Service.UpdateCardAndVoucher(giftVoucher);

            return result;
        }

        [Route("GetAmountByCVName")]
        [HttpGet]
        public List<RptGiftCardAndVoucher> GetAmountByCVName(string name)
        {
            try
            {
                var ItemList = _Service.GetAmountByCVName(name);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }
    }
}
    


