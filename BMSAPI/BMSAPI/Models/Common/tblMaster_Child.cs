namespace BMSAPI.Models.Common
{
    public class MasterChild:Base
    {
        public int ChildId { get; set; }
        public string? Value { get; set; }
        public string? ChildName { get; set; }
        public string? ParentId { get; set; }
    }

}
