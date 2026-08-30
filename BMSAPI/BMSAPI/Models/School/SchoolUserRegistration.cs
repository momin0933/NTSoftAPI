namespace BMSAPI.Models.School
{
    public class SchoolUserRegistration:Base
    {
        public string? Name { get; set; }
        public string? BName { get; set; }
        public string? Phone { get; set; }
        public string? Batch { get; set; }
        public string? RollSection { get; set; }
        public string? Division { get; set; }
        public string? Category { get; set; }
        public int Guest { get; set; }
        public string? PresentAddress { get; set; }
        public string? PremanetAddress { get; set; }
        public string? ImgPath { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TranID { get; set; }
        public string? SpecialNote { get; set; }
        public string? Status { get; set; }
        public string? RegNo { get; set; }
    }

    public class RegistrationResult
    {
        public bool Success { get; set; }
        public int Id { get; set; }
    }
}
