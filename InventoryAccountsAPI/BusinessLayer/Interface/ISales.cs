
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface ISales
    {
        
        public List<RptItemsForSales> GetAllItemsForSale(int whId);
        public List<RptItemsForSales> GetItemByBarCode(int whId, string barCode);
        public List<RptSalesInvoice> GetSalesReport(DateTime fromDate, DateTime todate, int whId);
        public List<RptDueCollection> GetDueCollection(int? customerId = null, int? invoiceId = null);

        public List<RptPaymentHistory> GetPaymentHistory(DateTime? fromDate = null, DateTime? toDate = null, int? customerId = null);
        public List<RptDueCollection> GetCustoerTotalDue();

        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNo(string invoiceNo, int id);
        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNoReturn(string invoiceNo);
        public List<RptInvoiceDetails> GetInvoiceDueByInvoiceNo(string invoiceNo);

        public List<SalesReport> SalesReport(int whId,DateTime FromDt, DateTime ToDt);
        public List<SalesChartReport> SalesChartReport(int whId, DateTime FromDt, DateTime ToDt);
        public List<SalesChartReport> MonthlySalesChartReport(int whId, DateTime FromDt, DateTime ToDt);
        public List<RptBrandWiseSalesReport> BrandWiseSalesReport(int whId, DateTime FromDt, DateTime ToDt);
        public List<RptBrandWiseSalesReport> FilterSalesReport(int whId, DateTime FromDt, DateTime ToDt);

        public int CountInvoice(string compCode);

        List<RptSalesManSalesReport> SalesManSalesReport(int whId,DateTime fromDt,DateTime toDt,string? salesMan);
        List<RptSalesManSalesReport> SalesManSalesReportByBarcode(int whId,  string? barCode);

    }
}
