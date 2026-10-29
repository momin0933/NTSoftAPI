
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.Common;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IItemReceive : ICommonMethod<ItemReceive>
    {
        public int AddAndUpdate(ItemReceive entity, string paymentType);
        public int AddAndUpdateNew(ItemReceive entity, string paymentType, int? paymentLedgerId, string voucherNumber);
        //public List<ItemRecievewWithItemDetails> GetItemRecieveWithItemDetails(int wareHouse);
        public int UpdateForEdit(ItemReceivePerams entity);
        public List<RtpReceiveItemList> GetAllReceivedItemsByRcvId(int rcvId);
        //public int SellingPriceUpdate(PriceUpdateRequest entity);

        public List<string> LastVoucherNoWithPrefix(string typeString);

    }
}
