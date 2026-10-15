
namespace BMSAPI.Models
{
    public class VwServiceCharge
    {
        public int? Id { get; set; }
        public int? OwnerId { get; set; }
        public int? FlatId { get; set; }
        public string? Name { get; set; }
        public string? Flat { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Religion { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? NID { get; set; }
        public string? FlatCode { get; set; }    
        public string? ImgPath { get; set; }
        public string? Building { get; set; }
        public int? BuildingId { get; set; }
        public string? FlatWiseServiceCharge { get; set; }
        public decimal  Collection { get; set; }
        public decimal DueAmount { get; set; }
        public string? IsBilling { get; set; }
        public int? TotalFlat { get; set; }
        public int? TotalMosquUsingFlat { get; set; }
        public string? UpdateBy { get; set; }
        public string? TntName { get; set; }
        public string? TntPhone { get; set; }
        public string? TntEmail { get; set; }
        public string? TntReligion { get; set; }
        public string? TntGender { get; set; }
        public string ?TntNID { get; set; }
        public string? TntAddress { get; set; }
        public DateTime? OwnershipStartDate { get; set; }
        public DateTime? BillDate { get; set; }
        public DateTime DueCollectionDate { get; set; }
        public bool IsMosqueBillGenerate { get; set; }
        public virtual ICollection<RptExpense>? ExpenseList { get; set; }
        public virtual ICollection<RptGasExpense>? GasExpenseList { get; set; }
        public virtual ICollection<RptExtraCharge>? ExtraChargeList { get; set; }



    }
}
