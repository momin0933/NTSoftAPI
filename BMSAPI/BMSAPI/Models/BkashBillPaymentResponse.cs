namespace BMSAPI.Models
{
    public class BkashBillPaymentResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public string ConsumerName { get; set; }
        public string TotalAmount { get; set; }
        public string TrxId { get; set; }
        public string MiddlewarePayTime { get; set; }   

    }
}
