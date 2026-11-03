
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISetting _ISetting;

        public SettingController(ISetting ISetting)
        {
            _ISetting = ISetting;
        }

        [HttpPost]
        public int Post([FromBody] StockAdjustment entity)
        {
            _ISetting.Add(entity);
            return entity.Id;
        }

        [HttpGet]
        public List<StockAdjustment> GetAll(int WhId)
        {
            var list = _ISetting.GetAll().Where(x => x.IsActive == true && x.WhId == WhId).ToList();
            return list;
        }
        [Route("AddStockAdjustment")]
        [HttpPost]
        public int AddStockAdjustment([FromBody] List<StockAdjustmentModel> entity)
        {
            _ISetting.AddStockAdjustment(entity);
            return entity.Count;
        }
        [Route("GetAdjustmentItemWithActualstock")]
        [HttpGet]
        public List<FinalStockAdjustment> GetAdjustmentItemWithstock(int WhId)
        {
            var list = _ISetting.GetAdjutmentItemWithActualStock(WhId);
            return list;
        }

        [Route("AddFinalStockAdjustment")]
        [HttpPost]
        public int AddFinalAdjustment([FromBody] List<FinalStockAdjustment> entity)
        {
            _ISetting.AddFinalAdjustment(entity);
            return entity.Count;
        }

        [Route("GetVariantByBarcode")]
        [HttpGet]
        public List<RptItem> GetVariantByBarcode(string barcode)
        {
            var list = _ISetting.GetVariantByBarcode(barcode);
            return list;
        }

        [Route("InventoryAdjustment")]
        [HttpPost]
        public int InventoryAdjustment([FromBody] InventoryAdj entity)
        {
            _ISetting.InventoryAdjustment(entity);
            return entity.Id;
        }
    }
}
