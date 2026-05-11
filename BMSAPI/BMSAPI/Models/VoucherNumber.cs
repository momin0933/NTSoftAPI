namespace BMSAPI.Models
{
    public class VoucherNumber
    {
        public int LedgerId { get; set; }
        public int CollectorLedgerId { get; set; }
        public string? LastVoucherNumber { get; set; }
        public string? LdgAccNo { get; set; }
        public string? Code { get; set; }
    }
}
