using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTSoftMerchantAPI.Models
{
    
    public class Order : Base
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

        public DateTime? OrderDate { get; set; }
        public DateTime? BuyerShipDate { get; set; }
        public DateTime? FactoryShipDate { get; set; }

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

        [ForeignKey("ShipmentTerms")]
        public int? ShipmentTermsId { get; set; }

        [ForeignKey("ShippingMode")]
        public int? ShippingModeId { get; set; }

        [ForeignKey("PaymentMode")]
        public int? PaymentModeId { get; set; }

        public int? TotalOrderQty { get; set; }
        public decimal? BuyerPrice { get; set; }
        public decimal? FactoryPrice { get; set; }

        [StringLength(100)]
        public string? ImagePath { get; set; }

        [StringLength(100)]
        public string? ImageName { get; set; }
    }
}