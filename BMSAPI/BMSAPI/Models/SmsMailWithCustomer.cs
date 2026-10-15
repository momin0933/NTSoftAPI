using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class SmsMailWithCustomer:Base
    {
         //[ForeignKey("tblSmsMail")]
        [ForeignKey(nameof(CustomerSmsMail))]
        [Column("SMSMailId")]
        public int SMSMailId { get; set; }

        public string? Phone { get; set; }

        // Add this navigation property
        public virtual CustomerSmsMail CustomerSmsMail { get; set; } = null!;
    }
}
