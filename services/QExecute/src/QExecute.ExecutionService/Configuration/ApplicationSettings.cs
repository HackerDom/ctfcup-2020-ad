namespace QueenOfHearts.ExecutionService.Configuration
{
    public class ApplicationSettings
    {
        public string OpLogFileName = "./data/oplog";
        public string SnapshotFileName { get; set; } = "./data/snapshot";
        public string VictimStorageAddress { get; set; } = "storage";
        public int VictimStoragePort { get; set; } = 8000;
        public int Port { get; } = 8001;
    }
}
