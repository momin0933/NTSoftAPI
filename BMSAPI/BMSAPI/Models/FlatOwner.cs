using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class FlatOwner:Base
    {
        [ForeignKey("OwnerId")]
        public int? OwnerId { get; set; }
        public int? FlatId { get; set; }
        public int? BuildingId { get; set; }
        public bool? IsMosqueBillGenerate { get; set; }
        public DateTime? OwnershipStartDate { get; set; }
        public DateTime? OwnershipEndDate { get; set; }
        public string? DeedCopy { get; set; }
        public Owner? Owner { get; set; }
        public string? TntName { get; set; }
        public string? TntPhone { get; set; }
        public string? TntEmail { get; set; }
        public string? TntAddress { get; set; }
        public string? TntGender { get; set; }
        public string? TntReligion { get; set; }
        public string? TntNID { get; set; }
       
    }
}
