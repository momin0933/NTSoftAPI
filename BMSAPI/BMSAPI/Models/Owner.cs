namespace BMSAPI.Models
{
    public class Owner :Base
    {
        public string? Name { get; set; }
        public string? ImgPath { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? NID { get; set; }
        public string? Religion { get; set; }
        public ICollection<FlatOwner>? FlatOwners { get; set; }
    }
}
