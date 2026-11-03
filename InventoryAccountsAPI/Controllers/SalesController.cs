
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISales _ISales;
        public SalesController(ISales sales)
        {
            _ISales = sales;
        }
        [Route("GetAllItemsForSales")]
        [HttpGet]
        public List<RptItemsForSales> GetAllItemsForSales(int whId)
        {
            try
            {
                List<RptItemsForSales> salesItemList = _ISales.GetAllItemsForSale(whId).ToList();
                return salesItemList;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Route("GetItemByBarCode")]
        [HttpGet]
        public List<RptItemsForSales> GetItemByBarCode(int whId, string barCode)
        {
            try
            {
                List<RptItemsForSales> list = _ISales.GetItemByBarCode(whId, barCode).ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Route("GetSalesReport")]
        [HttpGet]
        public List<RptSalesInvoice> GetSalesReport(DateTime fromDate, DateTime todate, int whId)
        {
            try
            {
                var salesReport = _ISales.GetSalesReport(fromDate, todate, whId).OrderByDescending(x => x.InvoiceDt).ToList();
                return salesReport;

            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetDueCollection")]
        public IActionResult GetDueCollection(int? customerId = null, int? invoiceId = null)
        {
            try
            {
                var result = _ISales.GetDueCollection(customerId, invoiceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetPaymentHistory")]
        public IActionResult GetPaymentHistory(int? customerId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _ISales.GetPaymentHistory(fromDate, toDate, customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("GetCustomerTotalDue")]
        public IActionResult GetCustomerTotalDue()
        {
            try
            {
                var result = _ISales.GetCustoerTotalDue();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Route("GetInvoiceDetailsByInvoiceNo")]
        [HttpGet]
        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNo(string invoiceNo, int id)
        {
            var invoiceDetailsList = _ISales.GetInvoiceDetailsByInvoiceNo(invoiceNo, id).ToList();
            return invoiceDetailsList;
        }

        [Route("GetInvoiceDetailsByInvoiceNoReturn")]
        [HttpGet]
        public List<RptInvoiceDetails> GetInvoiceDetailsByInvoiceNoReturn(string invoiceNo)
        {
            var invoiceDetailsList = _ISales.GetInvoiceDetailsByInvoiceNoReturn(invoiceNo).ToList();
            return invoiceDetailsList;
        }

        [Route("GetInvoiceDueByInvoiceNo")]
        [HttpGet]
        public List<RptInvoiceDetails> GetInvoiceDueByInvoiceNo(string invoiceNo)
        {
            var invoiceDetailsList = _ISales.GetInvoiceDueByInvoiceNo(invoiceNo).ToList();
            return invoiceDetailsList;
        }

        [Route("SalesReport")]
        [HttpGet]
        public List<SalesReport> SalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            var salesReport = _ISales.SalesReport(whId, FromDt, ToDt).ToList();
            return salesReport;
        }


        [Route("SalesChartReport")]
        [HttpGet]
        public List<SalesChartReport> SalesChartReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            var salesReport = _ISales.SalesChartReport(whId, FromDt, ToDt).ToList();
            return salesReport;
        }

        [Route("MonthlySalesChartReport")]
        [HttpGet]
        public List<SalesChartReport> MonthlySalesChartReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            var salesReport = _ISales.MonthlySalesChartReport(whId, FromDt, ToDt).ToList();
            return salesReport;
        }

        [Route("BrandWiseSalesReport")]
        [HttpGet]
        public List<RptBrandWiseSalesReport> BrandWiseSalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            var salesReport = _ISales.BrandWiseSalesReport(whId, FromDt, ToDt).ToList();
            return salesReport;
        }
        [Route("FilterSalesReport")]
        [HttpGet]
        public List<RptBrandWiseSalesReport> FilterSalesReport(int whId, DateTime FromDt, DateTime ToDt)
        {
            var salesReport = _ISales.FilterSalesReport(whId, FromDt, ToDt).ToList();
            return salesReport;
        }


        [HttpGet]
        [Route("GetInvoiceCount")]
        public int GetInvoiceCount(string compCode)
        {
            int invoiceCount = _ISales.CountInvoice(compCode);
            return invoiceCount;
        }

        [HttpGet]
        [Route("SalesManSalesReport")]
        public IActionResult SalesManSalesReport(int whId, DateTime fromDt, DateTime toDt, string? salesMan = null)
        {
            try
            {
                var data = _ISales.SalesManSalesReport(whId, fromDt, toDt, salesMan);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("SalesManSalesReportByBarcode")]
        public IActionResult SalesManSalesReportByBarcode(int whId, string? barCode = null)
        {
            try
            {
                var data = _ISales.SalesManSalesReportByBarcode(whId, barCode);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
