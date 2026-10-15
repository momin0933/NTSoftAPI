namespace BMSAPI.Models
{
    public class MenuViewModel
    {
        public int? MenuId { get; set; }
        public string? DisplayName { get; set; }
        public string? ActionResult { get; set; }
        public string? Controller { get; set; }
        public string? MenuIcon { get; set; }
        public int ModuleId { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? ProjectName { get; set; }
        public string? DisplayOrder { get; set; }
    }

    public class CompanyWiseMenuUpdate
    {
        public int MenuId { get; set; }
        public int CompId { get; set; }
        public bool IsActive { get; set; }
        public int ModuleId { get; set; }
        public string? UserRole { get; set; }
        public DateTime EntryDate { get; set; }
        public string? EntryBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
    }

}
