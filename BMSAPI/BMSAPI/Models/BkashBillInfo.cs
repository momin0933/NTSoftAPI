namespace BMSAPI.Models
{
    public class BkashBillInfo
    {
        public string? ErrorCode { get; set; }
        public string? ErrorMsg { get; set; }
        public string? FlatCode { get; set; }
        //public string? OwnerName { get; set; }
        public string? ConsumerName { get; set; }
        //public string? DueDate { get; set; }
        public string? BillDueDate { get; set; }
        public string? Status { get; set; }
        public string? QueryTime { get; set; }
        public string? BillAmount { get; set; }
        public string? BillMonth { get; set; }
    }
}
