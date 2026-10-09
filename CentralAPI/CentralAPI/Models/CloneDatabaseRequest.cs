namespace CentralAPI.Models
{
    public class CloneDatabaseRequest
    {
        public string NewDatabaseName { get; set; }
        public string SourceDatabaseName { get; set; }

        // "full"   = copy everything
        // "custom" = copy everything, then remove objects that are not in KeepObjectIds
        public string Mode { get; set; } = "full";

        // object_id values from GET objects (restore keeps the same object_id values)
        public List<int> KeepObjectIds { get; set; }
    }
}
