namespace QueenOfHearts.ExecutionService.Configuration
{
    public class ApplicationSettings
    {
        public string OpLogFileName = "oplog";
        public string SnapshotFileName { get; set; } = "snapshot";
        public string VictimStorageAddress { get; set; } = "127.0.0.1";
        public int VictimStoragePort { get; set; } = 8000;
        public int Port { get; } = 8001;
    }
}