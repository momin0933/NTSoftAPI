namespace BMSAPI.Models.Apps.PropHUB
{
    public class TogglePropertyStatusRequest
    {
        public int PropertyId { get; set; }
        public bool IsActive { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
