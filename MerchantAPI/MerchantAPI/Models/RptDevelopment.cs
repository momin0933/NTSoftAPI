using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MerchantAPI.Models
{
    public class RptDevelopment:Base
    {
        public int? BuyerId { get; set; }
        public string? BName { get; set; }

        public int? CustomerId { get; set; }
        public string? CName { get; set; }

        public int? FactoryId { get; set; }
        public string? FName { get; set; }

        public int? OrderSeasonId { get; set; }
        public string? SName { get; set; }

        public int? OrderTypeId { get; set; }
        public string? OTName { get; set; }

        public int? DepartmentId { get; set; }
        public string? DName { get; set; }
        public int? CategoryId { get; set; }
        public string? CaName { get; set; }
        public DateTime? InqDate { get; set; }

        [StringLength(500)]
        public string? PurchaseOrder { get; set; }

        [StringLength(500)]
        public string? StyleNo { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? FabricDescription { get; set; }

        [ForeignKey("Destination")]
        public int? DestinationId { get; set; }

        public int? TotalOrderQty { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? OfferPrice { get; set; }

        [StringLength(100)]
        public string? ImagePath { get; set; }

        [StringLength(100)]
        public string? ImageName { get; set; }

    }
}
