namespace BMSAPI.Models
{
    public class MasterChildItem
    {
        public int ChildId { get; set; }
        public int? ParentId { get; set; }
        public string? ChildName { get; set; }
    }
}
