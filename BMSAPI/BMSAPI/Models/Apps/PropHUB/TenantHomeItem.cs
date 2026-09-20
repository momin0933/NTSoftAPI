namespace BMSAPI.Models.Apps.PropHUB
{
    public class TenantHomeItem:Base
    {
        public int UId { get; set; }
        public string? PropertyName { get; set; }
        public string? Address { get; set; }
        public string? Area { get; set; }
        public string? UnitNo { get; set; }
        public decimal? MonthlyRent { get; set; }
        public int? RentDueDay { get; set; }
        public DateTime? MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }
        public string? Status { get; set; }
        public string? ConnectionStatus { get; set; }
        public int? LinkedTenancyId { get; set; }
    }
}
