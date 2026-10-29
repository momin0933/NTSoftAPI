
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTSoftAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTransferController : ControllerBase
    {

        private readonly IItemTransfer _itemTransfer;

        public ItemTransferController(IItemTransfer itemTransfer)
        {
            _itemTransfer = itemTransfer;

        }

        [HttpPost]
        public int Post([FromBody] ItemTransfer entity)
        {
            _itemTransfer.Add(entity);
            _itemTransfer.UpdateStockAndItem(entity);
            return entity.Id;
        }
        [HttpGet]
        public List<ItemTransfer> GetAll()
        {
            var trasferList = _itemTransfer.GetAll();
            return trasferList;
        }

        [Route("GetTransferItemWithDetails")]
        [HttpGet]
        public List<ItemTransferWithDetails> GetTransferItemWithDetails(string transferNo)
        {
            var transferItemDeatilList = _itemTransfer.GetItemTransferWithDetails(transferNo);

            return transferItemDeatilList;
        }

        //[Route("TransferItemReceive")]
        //[HttpPut]
        //public int TransferItemReceive(ItemTransfer entity)
        //{
        //    _itemTransfer.UpdateTransferItemDetails(entity);
        //    return entity.Id;
        //}
        [Route("GetAllReceiveTransferItem")]
        [HttpGet]
        public List<RptItemTransfer> GetAllReceiveTransferItem(int whId, DateTime FromDt, DateTime ToDt, string Status)
        {
            var itemTrasferReceiveList = _itemTransfer.GetAllReceiveTransferItem(whId, FromDt, ToDt, Status);
            return itemTrasferReceiveList;
        }
        [Route("GetAllTransferItem")]
        [HttpGet]
        public List<ItemTransfer> GetAllTransferItem(int whId)
        {
            var itemTrasferReceiveList = _itemTransfer.GetAllTransferItemByWareHouse(whId);
            return itemTrasferReceiveList;
        }

        [Route("GetReceiveItemByReceiveNo")]
        [HttpGet]
        public List<ItemNameList> GetReceiveItemByReceiveNo(string ReceiveNo)
        {
            var itemTrasferReceiveList = _itemTransfer.GetReceiveItemByReceiveNo(ReceiveNo);
            return itemTrasferReceiveList;
        }

        // [Route("GetAllReceiveTransfer")]
        //[HttpGet]
        //public List<ItemTransfer> GetAllReceiveTransfer(DateTime fromDate, DateTime toDate)
        //{
        //    //var list = _itemTransfer.GetAll()
        //    //     .Where(x => x.TransferDt >= fromDate
        //    //              && x.TransferDt <= toDate )
        //    //     .OrderByDescending(a => a.TransferDt)
        //    //     .ToList();

        //    //var list = (from itemTransfer in _itemTransfer
        //    //            join warehouse in _context.Warehouses
        //    //            on itemTransfer.WarehouseId equals warehouse.WarehouseId
        //    //            where itemTransfer.TransferDt >= fromDate && itemTransfer.TransferDt <= endDate
        //    //            orderby itemTransfer.TransferDt descending
        //    //            select new TransferWithWarehouse
        //    //            {
        //    //                TransferId = itemTransfer.TransferId,
        //    //                TransferDt = itemTransfer.TransferDt,
        //    //                WarehouseName = warehouse.WarehouseName,
        //    //                // Include other relevant fields here
        //    //            }).ToList();

        //    return list;
        //}

    }
}
