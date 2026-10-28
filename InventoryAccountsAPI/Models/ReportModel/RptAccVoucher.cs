using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class RptAccVoucher:Base
    {
        public string? VoucherType { get; set; }
        public string? VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Narration { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? VoucherStatus { get; set; }
        public int? CompanyId { get; set; }
        public int? UnitId { get; set; }
        public int? TeamId { get; set; }
        public  ICollection<RptVoucherDetails>? rptVoucherDetails { get; set; }
    }
    public class RptVoucherDetails: Base
    {
        public string? LedgerName { get; set; }
        public int AccVoucherId { get; set; }
        public int? LedgerId { get; set; }
        public string? ShortDesc { get; set; }
        public decimal? Amount { get; set; }
        public string? TranType { get; set; }
        public string? PaymentType { get; set; }
        public string? ChequeNo { get; set; }
        public string? BankNbranch { get; set; }
        public DateTime? ChequeDate { get; set; }

    }
}
