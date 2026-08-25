namespace BMSAPI.Models.Apps.PropHUB
{
    public class BillView
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public int? PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? TenantPhone { get; set; }
        public string? BillMonth { get; set; }
        public string? BillYear { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public System.DateTime? CollectionDate { get; set; }
    }
}
