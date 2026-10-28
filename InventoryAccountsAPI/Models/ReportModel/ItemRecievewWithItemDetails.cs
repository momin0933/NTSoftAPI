using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class ItemRecievewWithItemDetails
    {
        public int ReceiveId { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public DateTime ReceiveDt { get; set; }
        public string ReceiveNo { get; set; }
        public int WhId { get; set; }
        public int TotalQty { get; set; }
        public string Remarks { get; set; }
        public DateTime TranDt { get; set; }
        public int Qty { get; set; }
        public string Status { get; set; }
        public string TransactionRemarks { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string EntryBy { get; set; }
        public string UpdateBy { get; set; }
        public bool IsActive { get; set; }

    }
}
