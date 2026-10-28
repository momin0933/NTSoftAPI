using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptDiscount
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal? Value { get; set; }
        public string? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CompanyName { get; set; }
        public int? DiscountId { get; set; }
        //public int? ItemId { get; set; }
        public int? WhId { get; set; }
        public int? VariantId { get; set; }
        public int? Qty { get; set; }
    }
}
