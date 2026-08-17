namespace BMSAPI.Models.Apps.PropHUB
{
    public class PropertyDetails:Base
    {
        public string? Phone { get; set; }
        public int? PropertyId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public int? Room { get; set; }
        public int? Bathroom { get; set; }
        public int? Balcony { get; set; }
        public string? MeterNo { get; set; }
    }
}
