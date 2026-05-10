namespace BMSAPI.Models
{
    public class VbntblBkashPayments : Base
    {
        public int Id { get; set; }
        public string FlatCode { get; set; }
        public string BillNo { get; set; }
        public string BillMonth { get; set; }
        public decimal Amount { get; set; }
        public string UserMobileNumber { get; set; }
        public string TrxId { get; set; }
        public string PayTime { get; set; }
    }
}
