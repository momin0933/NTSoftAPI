namespace BMSAPI.Models
{
    public class ProcedureExecuteRequest
    {
        public string? Operation { get; set; } // "READ", "INSERT", "UPDATE", "DELETE"
        public string? ProcedureName { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
    }
}
