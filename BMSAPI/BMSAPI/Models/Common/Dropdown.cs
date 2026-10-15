namespace BMSAPI.Models.Common
{
    public class Dropdown
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
    }

    public class ReceiveNoList
    {
        public int Id { get; set; }
        public string? ReceiveNo { get; set; }
    }
}
