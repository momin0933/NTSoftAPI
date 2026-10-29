
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTSoftAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemReturnController : ControllerBase
    {
        private readonly IItemReturn _IItemReturn;
        public ItemReturnController(IItemReturn itemReturn)
        {
            _IItemReturn = itemReturn;
        }
        [HttpGet]
        [Route("GetAll")]
        public List<POSItemReturn> GetAll()
        {
            var list = _IItemReturn.GetAll();
            return list;
        }
        [HttpGet]
        [Route("GetById")]
        public POSItemReturn Get(int id)
        {
            var ItemRetun = _IItemReturn.GetById(id);
            return ItemRetun;
        }
        [HttpPost]
        [Route("Add")]
        public int Add(POSItemReturn entity)
        {
            _IItemReturn.Add(entity);
            return entity.Id;

        }
        [HttpPut]
        [Route("Update")]
        public int Update(POSItemReturn entity)
        {
            _IItemReturn.Update(entity);
            return entity.Id;
        }
        //[HttpDelete]
        //[Route("Delete")]
        //public int Delete(int id)
        //{
        //    _IItemReturn.Delete(id);
        //    return id;
        //}

        [HttpGet]
        [Route("GetReturnInvoiceCount")]
        public int GetReturnInvoiceCount(string compCode)
        {
            int invoiceCount = _IItemReturn.CountReturnInvoice(compCode);
            return invoiceCount;
        }


        [Route("GetAllItemStockWithPrice")]
        [HttpGet]
        public List<RptItem> GetAllItemStockWithPrice([FromQuery] string WHId, string type)
        {
            try
            {
                var ItemList = _IItemReturn.GetAllItemStockListWithPrice(Convert.ToInt32(WHId), type);
                return ItemList;
            }
            catch
            {
                throw;
            }
        }
    }
}
