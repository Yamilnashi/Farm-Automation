namespace FarmToTableData.Models
{
    public class HistoryState
    {
        public int HistoryStateId { get; set; }
        public byte[] LastTemperatureReadingLsn { get; set; }
        public byte[] LastMoistureReadingLsn { get; set; }
        public byte[] LastSoilReadingLsn { get; set; }
        public byte[] LastStatusLsn { get; set; }
        public byte[] LastSentinelLsn { get; set; }
    }
}
