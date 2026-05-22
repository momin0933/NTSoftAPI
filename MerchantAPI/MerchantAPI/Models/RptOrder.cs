using System.ComponentModel.DataAnnotations;

namespace MerchantAPI.Models
{
    public class RptOrder : Base
    {
        public int? DevId { get; set; }

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

        public int? DestinationId { get; set; }
        public string? DestName { get; set; }
        public int? ShipmentTermsId { get; set; }
        public string? STName { get; set; }
        public int? ShippingModeId { get; set; }
        public string? SMName { get; set; }
        public int? PaymentModeId { get; set; }
        public string? PMName { get; set; }

        [StringLength(50)]
        public string? CategoryId { get; set; }
        public string? CaName { get; set; }

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

        public int? TotalOrderQty { get; set; }
        public decimal? BuyerPrice { get; set; }
        public decimal? FactoryPrice { get; set; }

        [StringLength(100)]
        public string? ImagePath { get; set; }

        [StringLength(100)]
        public string? ImageName { get; set; }
    }
}
