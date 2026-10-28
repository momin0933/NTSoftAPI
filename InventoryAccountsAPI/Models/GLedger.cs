using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class GLedger
    {
        public int? GLedgerId { get; set; }
        public string? GLedgerCode { get; set; }
        public string? GLedgerDescription { get; set; }
        public string? GLedgerAccType { get; set; }
        public List<ILedger>? ILedgers { get; set; } = new List<ILedger>();
    }
    public class ILedger
    {
        public int? ILedgerId { get; set; }
        public string? ILedgerCode { get; set; }
        public string? ILedgerDescription { get; set; }
        public int? GLedgerId { get; set; }
    }
}
