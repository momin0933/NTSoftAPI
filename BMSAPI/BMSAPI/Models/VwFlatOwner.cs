using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class VwFlatOwner : Base
    {
        public int? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? NID { get; set; }
        public string? Religion { get; set; }

        [ForeignKey("BuildingId")]
        public int? BuildingId { get; set; }
        public string? Floor { get; set; }
        public string? FlatNumber { get; set; }
        public string? FlatCode { get; set; }
        public string? FlatType { get; set; }
        public int? SizeSqft { get; set; }
    }
}