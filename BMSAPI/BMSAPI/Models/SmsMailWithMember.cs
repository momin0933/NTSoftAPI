using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class SmsMailWithMember : Base
    {
        [ForeignKey("CommitteeSmSMail")]
        [Column("SMSMailId")]
        public int SMSMailId { get; set; }

        public int MemberId { get; set; }

        // Add this navigation property
        public virtual CommitteeSmSMail CommitteeSmSMail { get; set; } = null!;
    }
}