namespace BMSAPI.Models.Common
{
    public class ChartOfAccount : Base
    {


        //public int Id { get; set; }
        public int OrgCode { get; set; }

        public string? AccCode { get; set; }
        public string? AccName { get; set; }
        public string? AccType { get; set; }
        public string? AccSource { get; set; }
        
       // public string? LevelTwoDescription { get; set; }
        //public string? LevelOneDescription { get; set; }
        public string? AccUsage { get; set; }

        public int LevelOneAutoID { get; set; }

        public int LevelTwoAutoID { get; set; }
        public string? CashFlow { get; set; }

        
        public string? CashFlowGroup { get; set; }

        public string? Status { get; set; }
        public string? CashType { get; set; }
        public string? EntryStatus { get; set; }
       
    }
}
