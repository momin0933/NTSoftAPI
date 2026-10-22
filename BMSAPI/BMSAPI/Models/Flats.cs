using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class Flats:Base
    {
        [ForeignKey("BuildingId")]
        public int? BuildingId { get; set; }
        public string? Floor { get; set; }
        public string? FlatNumber { get; set; }
        public string? FlatCode { get; set; }        
        public string? FlatType { get; set; }
        public int? SizeSqft { get; set; }
    }
}
