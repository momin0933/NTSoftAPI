using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptGiftCardAndVoucher
    {
         public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal? Value { get; set; }
        public string? Type { get; set; }
        public string? CVName { get; set; }
        public string? Status { get; set; }
        public int? Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? WhId { get; set; }
        public int? Qty { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
    }
}
