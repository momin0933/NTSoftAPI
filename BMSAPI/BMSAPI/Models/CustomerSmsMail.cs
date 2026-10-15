namespace BMSAPI.Models
{
    public class CustomerSmsMail:Base
    {
        public string? MailSub { get; set; }
        public string? Mail { get; set; }
        public string? MailAttatchPath { get; set; }
        public string? SMS { get; set; }

        // Navigation property
        public virtual ICollection<SmsMailWithCustomer> Members { get; set; } = new List<SmsMailWithCustomer>();
    }
}
