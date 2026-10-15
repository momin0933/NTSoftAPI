namespace BMSAPI.Models.Common
{
    public class POSItemReturn :Base
    {
        public int InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public string?  ReturnNo{ get; set; }
        public  DateTime? ReturnDt{ get; set; }
        public virtual ICollection<POSItemReturnDetails>? POSItemReturnDetails { get; set; }
    }
}
