using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class SalesManager : ISales
    {
        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        public SalesManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }
        public List<RptItemsForSales> GetAllItemsForSale(int whId)
        {
            string SP = "SP_ItemsForSales";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            p.Add("@WhId", whId);
            List<RptItemsForSales> ItemList = _IDapperService.GetAllBySP<RptItemsForSales>(SP, p).ToList();
            return ItemList;
        }


        public List<RptItemsForSales> GetItemByBarCode(int whId, string barCode)
        {
            string SP = "SP_ItemsForSales";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            p.Add("@WhId", whId);
            p.Add("@barCode", barCode);

            List<RptItemsForSales> ItemList = _IDapperService.GetAllBySP<RptItemsForSales>(SP, p).ToList();
            return ItemList;
        }

        public List<RptSalesInvoice> GetSalesReport(DateTime fromDate, DateTime todate, int whId)
        {
            try 
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 1);
                p.Add("@FromDt", fromDate);
                p.Add("@ToDt", todate);
                p.Add("@WhId", whId);
                List<RptSalesInvoice> salesReportList = _IDapperService.GetAllBySP<RptSalesInvoice>(SP, p).ToList();
                return salesReportList;

            }
            catch 
            {
                throw;
            }

        }
        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNo(string invoiceNo, int id)
        {
            try {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 2);
                p.Add("@invoiceNo", invoiceNo);
                p.Add("@InvoiceId", id);
                List<RptInvoiceDetails> rptInvoiceDetails = _IDapperService.GetAllBySP<RptInvoiceDetails>(SP, p).ToList();
                return rptInvoiceDetails;
            }
            catch {
                throw;
            }

        }

        public List<SalesReport> SalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            try
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 4);
                p.Add("@whId", whId);
                p.Add("@@FromDt", FromDt);
                p.Add("@ToDt", ToDt);
                List<SalesReport> salesReport = _IDapperService.GetAllBySP<SalesReport>(SP, p).ToList();
                return salesReport;
            }
            catch
            {
                throw;
            }
        }

        public List<SalesChartReport> SalesChartReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            try
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 5);
                p.Add("@whId", whId);
                p.Add("@@FromDt", FromDt);
                p.Add("@ToDt", ToDt);
                List<SalesChartReport> salesReport = _IDapperService.GetAllBySP<SalesChartReport>(SP, p).ToList();
                return salesReport;
            }
            catch
            {
                throw;
            }
        }

        public List<RptBrandWiseSalesReport> BrandWiseSalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            try
            {
                
                    string SP = "SP_SalesInvoice";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@Querychecker", 6);
                    p.Add("@whId", whId);
                    p.Add("@@FromDt", FromDt);
                    p.Add("@ToDt", ToDt);
                    List<RptBrandWiseSalesReport> salesReport = _IDapperService.GetAllBySP<RptBrandWiseSalesReport>(SP, p).ToList();
                    return salesReport;
              
            }
            catch
            {
                throw;
            }
        }

        public List<RptBrandWiseSalesReport> FilterSalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            string SP = "SP_SalesInvoice";
            DynamicParameters p = new DynamicParameters();
            p.Add("@Querychecker", 7);
            p.Add("@whId", whId);
            p.Add("@@FromDt", FromDt);
            p.Add("@ToDt", ToDt);
            List<RptBrandWiseSalesReport> salesReport = _IDapperService.GetAllBySP<RptBrandWiseSalesReport>(SP, p).ToList();
            return salesReport;
        }

        public List<RptDueCollection> GetDueCollection(int? customerId = null, int? invoiceId = null)
        {
            string SP = "SP_DueCollection";
            DynamicParameters p = new DynamicParameters();

            p.Add("@QueryChecker", 1);
            p.Add("@CustomerId", customerId);  
            p.Add("@InvoiceId", invoiceId);     

            return _IDapperService.GetAllBySP<RptDueCollection>(SP, p).ToList();
        }


        public List<RptInvoiceDetails> GetInvoiceDueByInvoiceNo(string invoiceNo)
        {
            try
            {
                string SP = "SP_DueCollection";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 2);
                p.Add("@invoiceNo", invoiceNo);
                List<RptInvoiceDetails> rptInvoiceDetails = _IDapperService.GetAllBySP<RptInvoiceDetails>(SP, p).ToList();
                return rptInvoiceDetails;
            }
            catch
            {
                throw;
            }
        }

        public List<RptDueCollection> GetCustoerTotalDue()
        {
            string SP = "SP_DueCollection";
            DynamicParameters p = new DynamicParameters();

            p.Add("@QueryChecker", 3);
            return _IDapperService.GetAllBySP<RptDueCollection>(SP, p).ToList();
        }

        public List<RptPaymentHistory> GetPaymentHistory(DateTime? fromDate = null, DateTime? toDate = null, int? customerId = null)
        {
            string SP = "SP_PaymentHistory";
            DynamicParameters p = new DynamicParameters();

            p.Add("@QueryChecker", 1);
            p.Add("@FromDate", fromDate);
            p.Add("@ToDate", toDate);
            p.Add("@CustomerId", customerId);

            return _IDapperService.GetAllBySP<RptPaymentHistory>(SP, p).ToList();
        }

        public List<SalesChartReport> MonthlySalesChartReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            try
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 10);
                p.Add("@whId", whId);
                p.Add("@@FromDt", FromDt);
                p.Add("@ToDt", ToDt);
                List<SalesChartReport> salesReport = _IDapperService.GetAllBySP<SalesChartReport>(SP, p).ToList();
                return salesReport;
            }
            catch
            {
                throw;
            }
        }

        //public int CountInvoice()
        //{
        //    string currentYearMonth = DateTime.Now.ToString("yyMM");

        //    var lastInvoice = _ICommonService.Query<Invoice>() // IQueryable
        //        .Where(i =>
        //            !string.IsNullOrEmpty(i.InvoiceNo) &&
        //            i.InvoiceNo.StartsWith("SI-") &&
        //            i.InvoiceNo.Length >= 100 &&
        //            i.InvoiceNo.Substring(3, 4) == currentYearMonth
        //        )
        //        .OrderByDescending(i => i.Id)
        //        .FirstOrDefault();

        //    if (lastInvoice != null)
        //    {
        //        string lastCounterStr = lastInvoice.InvoiceNo.Substring(lastInvoice.InvoiceNo.Length - 2, 2);
        //        if (int.TryParse(lastCounterStr, out int lastCounter))
        //            return lastCounter + 1;
        //    }

        //    return 1;
        //}
        //public int CountInvoice(string compCode)
        //{
        //    //string currentYearMonthDay = DateTime.Now.ToString("yyMMdd"); 
        //    string currentYearMonth = DateTime.Now.ToString("yyMM");

        //    var lastInvoice = _ICommonService.Query<Invoice>()
        //        .Where(i =>
        //            !string.IsNullOrEmpty(i.InvoiceNo) &&
        //             //i.InvoiceNo.StartsWith("SI" + currentYearMonth) 
        //             //i.InvoiceNo.StartsWith(compCode + currentYearMonth)
        //             i.InvoiceNo.StartsWith(compCode) &&                  // Match CompCode
        //              i.InvoiceNo.Contains(currentYearMonth)
        //        )
        //        .OrderByDescending(i => i.Id)
        //        .FirstOrDefault();

        //    if (lastInvoice != null)
        //    {
        //        // Extract counter: everything after SI-YYMMDD
        //        string counterStr = lastInvoice.InvoiceNo.Substring(11); // 3 + 6 = 9 → after SIYYMMDD
        //        if (long.TryParse(counterStr, out long lastCounter))
        //            return (int)(lastCounter + 1); // next counter
        //    }

        //    // First invoice today
        //    return 1;
        //}
        public int CountInvoice(string compCode)
        {
            string currentYearMonth = DateTime.Now.ToString("yyMM");

            using var ctx = _factory.CreateDbContext();

            var lastInvoice = ctx.Set<Invoice>()
                .Where(i =>
                    !string.IsNullOrEmpty(i.InvoiceNo) &&
                    i.InvoiceNo.StartsWith(compCode) &&
                    i.InvoiceNo.Contains(currentYearMonth))
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            if (lastInvoice != null)
            {
                // Invoice number-এর format অনুযায়ী position যাচাই করুন।
                if (lastInvoice.InvoiceNo.Length > 11)
                {
                    string counterStr = lastInvoice.InvoiceNo.Substring(11);

                    if (long.TryParse(counterStr, out long lastCounter))
                        return (int)(lastCounter + 1);
                }
            }

            return 1;
        }
        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNoReturn(string invoiceNo)
        {
            try
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 14);
                p.Add("@invoiceNo", invoiceNo);            
                List<RptInvoiceDetails> rptInvoiceDetails = _IDapperService.GetAllBySP<RptInvoiceDetails>(SP, p).ToList();
                return rptInvoiceDetails;
            }
            catch
            {
                throw;
            }
        }

        public List<RptSalesManSalesReport> SalesManSalesReport(int whId,DateTime fromDt,DateTime toDt,string? salesMan)
        {
            try
            {
                string SP = "SP_POSSalesManReport";

                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 1);
                p.Add("@WhId", whId);
                p.Add("@FromDt", fromDt);
                p.Add("@ToDt", toDt);
                p.Add("@SalesMan", salesMan);

                return _IDapperService
                    .GetAllBySP<RptSalesManSalesReport>(SP, p)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<RptSalesManSalesReport> SalesManSalesReportByBarcode(int whId, string? barCode)
        {
            try
            {
                string SP = "SP_POSSalesManReport";

                DynamicParameters p = new DynamicParameters();
                p.Add("@Querychecker", 2);
                p.Add("@WhId", whId);           
                p.Add("@Barcode", barCode);

                return _IDapperService
                    .GetAllBySP<RptSalesManSalesReport>(SP, p)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
