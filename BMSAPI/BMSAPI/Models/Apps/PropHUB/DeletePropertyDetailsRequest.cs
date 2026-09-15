namespace BMSAPI.Models.Apps.PropHUB
{
    public class DeletePropertyDetailsRequest
    {
        public int UId { get; set; }
        public int PropDetailsId { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
