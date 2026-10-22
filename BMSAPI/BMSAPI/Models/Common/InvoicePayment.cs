namespace BMSAPI.Models.Common
{
    public class InvoicePayment
    {
        public int AccountId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }
}
