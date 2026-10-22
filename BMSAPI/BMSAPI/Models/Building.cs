namespace BMSAPI.Models
{
    public class Building : Base
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public int? TotalFlats { get; set; }
        public int? MosqueBillGenerate { get; set; }
        public ICollection<Flats>? Flats { get; set; }
        public ICollection<VwFlatOwner>? FlatOwners { get; set; }


    }
}
