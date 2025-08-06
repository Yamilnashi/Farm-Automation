namespace FarmToTableData.Models
{
    public class SentinelStatusHistoryChange : HistoryChangeBase
    {
        public ESentinelStatus SentinelStatusCode { get; set; }
        private int SentinelStatusHistoryId { get; set; }
        public override int HistoryId => SentinelStatusHistoryId;
    }
}
