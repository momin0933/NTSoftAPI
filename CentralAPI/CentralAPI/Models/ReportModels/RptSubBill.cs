namespace CentralAPI.Models.ReportModels
{
    public class RptSubBill
    {
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDt { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentType { get; set; }
        public DateTime? PaymentDt { get; set; }
    }
}
