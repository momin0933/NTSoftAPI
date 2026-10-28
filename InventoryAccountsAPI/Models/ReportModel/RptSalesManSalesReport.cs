using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptSalesManSalesReport
    {
        public int Id { get; set; }

        public DateTime InvoiceDt { get; set; }

        public string InvoiceNo { get; set; }

        public string PaymentType { get; set; }

        public string CustomerNumber { get; set; }

        public string SalesMan { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal Qty { get; set; }

        public string StyleNo { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string ProductType { get; set; }

        public string Description { get; set; }

        public decimal? Vat { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        public string Barcode { get; set; }

        public string? SourceType { get; set; }
        public string? ProductionType { get; set; }



    }
}
