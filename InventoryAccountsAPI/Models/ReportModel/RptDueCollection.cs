using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptDueCollection
    {
        public int? Id { get; set; }

        public int? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? PayAmount { get; set; }
        public string? PaymentType { get; set; }
        public string? InvoiceNo { get; set; }
    }
}
