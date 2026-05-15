using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MerchantAPI.Models
{
    public class Development:Base
    {
        [ForeignKey("Buyer")]
        public int? BuyerId { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        [ForeignKey("Factory")]
        public int? FactoryId { get; set; }

        [ForeignKey("OrderSeason")]
        public int? OrderSeasonId { get; set; }

        [ForeignKey("OrderType")]
        public int? OrderTypeId { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }

        [StringLength(50)]
        public string? CategoryId { get; set; }

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
