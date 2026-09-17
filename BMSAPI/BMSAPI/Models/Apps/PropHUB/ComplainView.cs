namespace BMSAPI.Models.Apps.PropHUB
{
    public class ComplainView
    {
        public int Id { get; set; }
        public int? UId { get; set; }
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? TenantPhone { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public int? PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public bool? IsReadByAdmin { get; set; }
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
