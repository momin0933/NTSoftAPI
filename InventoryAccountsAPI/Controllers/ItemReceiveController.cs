
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.Common;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTSoftAPI.Controllers
{
    public class ItemReceiveController : ControllerBase
    {
        private readonly IItemReceive _IItemReceive;

        public ItemReceiveController(IItemReceive IItemReceive)
        {
            _IItemReceive = IItemReceive;
        }
        [Route("api/ItemReceive")]
        [HttpGet]
        public List<ItemReceive> GetAll([FromQuery] string Stauts = "all")
        {
            try
            {
                List<ItemReceive> ItemReceivelist = new List<ItemReceive>();
                if (Stauts == "all")
                {
                    ItemReceivelist = _IItemReceive.GetAll();
                }
                else if (Stauts == "true")
                {
                    ItemReceivelist = _IItemReceive.GetAll().Where(m => m.IsActive == true).ToList();
                }
                else
                {
                    ItemReceivelist = _IItemReceive.GetAll().Where(m => m.IsActive == false).ToList();
                }

                return ItemReceivelist;
            }
            catch
            {
                throw;
            }
        }

        //[Route("api/ItemReceive")]
        //[HttpPost]
        //public int Post([FromBody] ItemReceive entity, [FromQuery] string paymentType)
        //{
        //    _IItemReceive.AddAndUpdate(entity, paymentType);
        //    return entity.Id;
        //}
        [Route("ItemReceiveNew")]
        [HttpPost]
        public int ItemReceiveNew([FromBody] ItemReceive entity, [FromQuery] string paymentType, [FromQuery] int? paymentLedgerId, [FromQuery] string voucherNumber)
        {
            _IItemReceive.AddAndUpdateNew(entity, paymentType, paymentLedgerId, voucherNumber);
            return entity.Id;
        }
        [Route("api/ItemReceive")]
        [HttpPut]
        public int Put([FromBody] ItemReceive entity)
        {
            _IItemReceive.Update(entity);
            return entity.Id;
        }
        [Route("api/ItemReceive")]
        [HttpDelete]
        public int Remove([FromBody] ItemReceive entity)
        {
            _IItemReceive.Remove(entity);
            return entity.Id;
        }
        //[Route("api/GetItemRecieveWithItemDetails")]
        //[HttpGet]
        //public List<ItemRecievewWithItemDetails> GetItemRecieveWithItemDetails(int wareHouse)
        //{
        //    var list = _IItemReceive.GetItemRecieveWithItemDetails(wareHouse);
        //    return list;
        //}
        [Route("api/UpdateReceivedItem")]
        [HttpPut]
        public int UpdateReceivedItem([FromBody] ItemReceivePerams entity)
        {
            _IItemReceive.UpdateForEdit(entity);
            return entity.Id;
        }
        [Route("api/GetAllReceivedItem")]
        [HttpGet]
        public List<ItemReceive> GetAllReceivedItem(DateTime fromDate, DateTime toDate)
        {
            var list = _IItemReceive.GetAll()
                            .Where(x => x.ReceiveDt >= fromDate && x.ReceiveDt <= toDate)
                            .OrderByDescending(a => a.Id)
                            .ToList();
            return list;
        }
        [Route("api/GetAllReceivedItemByOutlet")]
        [HttpGet]
        public List<ItemReceive> GetAllReceivedItemByOutlet(DateTime fromDate, DateTime toDate, int whId)
        {
            var list = _IItemReceive.GetAll()
                            .Where(x => x.ReceiveDt >= fromDate
                                     && x.ReceiveDt <= toDate
                                     && x.WhId == whId)
                            .OrderByDescending(a => a.Id)
                            .ToList();
            return list;
        }

        [Route("api/GetAllReceivedItemWithOutStatus")]
        [HttpGet]
        public List<ItemReceive> GetAllReceivedItemWithOutStatus(DateTime fromDate, DateTime toDate, int whId)
        {
            var list = _IItemReceive.GetAll()
                            .Where(x => x.ReceiveDt >= fromDate
                                     && x.ReceiveDt <= toDate
                                     && x.WhId == whId)
                            .OrderByDescending(a => a.ReceiveDt)
                            .ToList();
            return list;
        }

        [Route("api/GetAllReceivedItemWithStatus")]
        [HttpGet]
        public List<ItemReceive> GetAllReceivedItemWithStatus(DateTime fromDate, DateTime toDate, int whId, string status)
        {
            var list = _IItemReceive.GetAll()
                            .Where(x => x.ReceiveDt >= fromDate
                                     && x.ReceiveDt <= toDate
                                     && x.WhId == whId
                                     && x.Status == status)
                            .OrderByDescending(a => a.Id)
                            .ToList();
            return list;
        }
        [Route("api/GetReceivedItemDetailsByRvcId")]
        [HttpGet]
        public List<RtpReceiveItemList> GetReceivedItemDetailsByRvcId(int rcvId)
        {
            var list = _IItemReceive.GetAllReceivedItemsByRcvId(rcvId).ToList();

            return list;
        }

        //[Route("api/UpdateSellingPrice")]
        //[HttpPost]
        //public int UpdateSellingPrice([FromBody] PriceUpdateRequest entity)
        //{
        //    _IItemReceive.SellingPriceUpdate(entity);
        //    return entity.Items.Count;
        //}

        [Route("api/VoucherNoWithPrefix")]
        [HttpGet]
        public List<string> VoucherNoWithPrefix(string prefix)
        {
            var list = _IItemReceive.LastVoucherNoWithPrefix(prefix);
            return list;

        }
    }
}
