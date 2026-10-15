namespace BMSAPI.Models
{
    public class CommitteeSmSMail : Base
    {
        public string? MailSub { get; set; }
        public string? Mail { get; set; }
        public string? MailAttatchPath { get; set; }
        public string? SMS { get; set; }

        // Navigation property
        public virtual ICollection<SmsMailWithMember> Members { get; set; } = new List<SmsMailWithMember>();
    }
}