using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptSupplierWiseReport
    {
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public int PurchaseQty { get; set; }
        public int TransferQty { get; set; }
        public int SellQty { get; set; }
        public int ReturnQty { get; set; }

    }
}
