using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class GiftVoucher : Base
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Amount { get; set; }
        public int Qty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WhId { get; set; }
        public virtual List<GiftVoucherDetails>? GiftVoucherDetails { get; set; }
    }

    public  class GiftVoucherDetails: Base
    {
        [ForeignKey("GiftVoucher")]
        public int GiftVoucherId { get; set; }
        public GiftVoucher? GiftVoucher { get; set; }
        public string CVName { get; set; }
        public string Status { get; set; }
        public string? InvoiceNo { get; set; }

    }
}
