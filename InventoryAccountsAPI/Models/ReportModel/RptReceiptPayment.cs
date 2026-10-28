using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class RptReceiptPayment
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public decimal NetBalance { get; set; }
        public int StatusOrder { get; set; }
        public string LedgerHead { get; set; }
        public string SortBy { get; set; }
        public int SortOrder { get; set; }
        public int CompanyId { get; set; }
        // public decimal ClosingBalance { get; set; }
        // public string GroupLedger { get; set; }
        // public decimal ReceiptBalance { get; set; }
        // public decimal PaymentBalance { get; set; }
    }
}
