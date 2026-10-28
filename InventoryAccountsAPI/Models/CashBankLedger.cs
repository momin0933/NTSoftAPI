using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class CashBankLedger:Base
    {
        public DateTime? TranDate { get; set; }
        public int? SourceId { get; set; }
        public string? SourceType { get; set; }
        public string? LedgerType { get; set; }
        public decimal? Amount { get; set; }
        public string? TransactionType { get; set; }
        public int AccountId { get; set; }
        public int LedgerId { get; set; }

    }
}
