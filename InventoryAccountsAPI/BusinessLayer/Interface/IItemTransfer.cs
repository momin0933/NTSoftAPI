
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IItemTransfer : ICommonMethod<ItemTransfer>
    {
        public string UpdateStockAndItem(ItemTransfer entity);
        public List<ItemTransferWithDetails> GetItemTransferWithDetails(string transferNo);
        // public int UpdateTransferItemDetails(ItemTransferReceive entity);
        public List<RptItemTransfer> GetAllReceiveTransferItem(int whId, DateTime FromDt, DateTime ToDt, string Status);
        public List<ItemTransfer> GetAllTransferItemByWareHouse(int whId);
        //public int UpdateTransferItemDetails(ItemTransfer entity);
        public List<ItemNameList> GetReceiveItemByReceiveNo(string ReceiveNo);

    }
}
