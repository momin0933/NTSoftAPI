using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptProfitLoss
    {
        public string? CompanyAddress { get; set; }
        public string? CompanyName { get; set; }
        public string? Title { get; set; }
        public string? Ledger { get; set; }

        public string? GroupLedger { get; set; }
        public string? AccType { get; set; }
        public string? TranType { get; set; }
        public string? LedgerId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ParentId { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
    }
}
