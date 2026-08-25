namespace BMSAPI.Models.Apps.PropHUB
{
    public class Bill:Base
    {
        public string? Phone { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public int? TenantId { get; set; }
        public string? BillMonth { get; set; }
        public string? BillYear { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? CollectionDate { get; set; }
    }
}
