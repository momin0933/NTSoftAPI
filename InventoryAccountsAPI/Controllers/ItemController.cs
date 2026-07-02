using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTSoftAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItem _Service;

        public ItemController(IItem item)
        {
            _Service = item;
        }
        [HttpGet]
        public List<RptItem> GetAll([FromQuery] int WhId)
        {
            try
            {
                List<RptItem> Itemlist = new List<RptItem>();
              
                    Itemlist = _Service.GetAllVariantList(WhId);
               

                return Itemlist;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetAllItemWithStock")]
        [HttpGet]
        public List<RptItem> GetAllItemWithStock([FromQuery] string WHId , string type)
        {
            try
            {
                var ItemList = _Service.GetAllItemListWithStock(Convert.ToInt32(WHId),type);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetAllItemWithStockByStyleNo")]
        [HttpGet]
        public List<RptItem> GetAllItemWithStockByStyleNo([FromQuery]  string styleNo, int WhId)
        {
            try
            {
                var ItemList = _Service.GetAllItemListWithStockByStyleNo(styleNo, WhId);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }

        //[HttpPost]
        //public int Post([FromBody] Item entity)
        //{
        //    _Service.Add(entity);
        //    return entity.Id;
        //}

        //[HttpPost("AddVariant")]
        //public IActionResult AddVariant([FromBody] List<ItemVariant> variants)
        //{
        //    try
        //    {
        //        int count = _Service.AddVariantToExistingItem(variants);
        //        return Ok($"{count} variant(s) added successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPut]
        //public int Put([FromBody] Item entity)
        //{
        //    _Service.Update(entity);
        //    return entity.Id;
        //}
        //[HttpDelete]
        //[Route("DeleteItemByID")]
        //public int DeleteItemByID(int id)
        //{
        //    _Service.DeleteById(id);
        //    return id;
        //}

        
        //[HttpGet]
        //[Route("GetItemCount")]
        //public string GetItemCount()
        //{
        //    string itemCount = _Service.CountItem();
        //    return itemCount;
        //}

        //[HttpGet]
        //[Route("GetItemNameById")]
        //public string GetItemNameById(int id)
        //{
        //    string name = _Service.GetItemNameById(id); 
        //    return name;
        //}
        //[HttpGet]
        //[Route("GetItemDetailsById")]
        //public VariantListForDropdown? GetItemDetailsById(int id)
        //{
        //    var item = _Service.GetItemDetailsById(id);
        //    return item;
        //}


        //[HttpGet]
        //[Route("GetSupplierWiseReport")]
        //public List<RptSupplierWiseReport> GetSupplierWiseData(DateTime fromDate, DateTime toDate)
        //{
        //    var item = _Service.GetSupplierWiseData(fromDate,toDate);
        //    return item;
        //}
        //[HttpPost]
        //[Route("UpdateItemNvariant")]
        //public int Put([FromBody] ItemVariantUpdate entity)
        //{
        //    _Service.UpdateItemVariant(entity);
        //    return entity.Id;
        //}
        //[Route("UpdateItem")]
        //[HttpPut]
        //public int UpdateProductItem(Item entity)
        //{
        //    return _Service.Update(entity);
        //}

        //[HttpPut("UpdateItemImage")]
        //public IActionResult UpdateItemImage([FromBody] Item entity)
        //{
        //    var result = _Service.UpdateItemImage(entity.Id, entity.ImgPath, entity.UpdateBy);
        //    return Ok(result);
        //}
    }
}
