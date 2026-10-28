namespace BMSAPI.Models
{
    public class SendMessage : Base
    {
        public string AppName { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
