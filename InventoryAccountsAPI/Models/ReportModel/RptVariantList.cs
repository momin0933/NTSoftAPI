using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptVariantList
    {
        public int? Id { get; set; }
        public int? ItemId { get; set; }
        public string? StyleNo { get; set; }
        public string? Brand { get; set; }
        public string ?Category { get; set; }
        public string? ProductType { get; set; }
        public string? Barcode { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? SellingPrice { get; set; }
    }
}
