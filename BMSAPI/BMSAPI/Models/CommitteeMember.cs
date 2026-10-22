namespace BMSAPI.Models
{
    public class CommitteeMember:Base
    {
        public int CId { get; set; }
        public int? OwnerId { get; set; }
        public int? FlatId { get; set; }
        public int? BuildingId { get; set; }
        public int? FlatCode { get; set; }
        public string? FlatNumber { get; set; }
        public string? Designation { get; set; } 

    }
}
