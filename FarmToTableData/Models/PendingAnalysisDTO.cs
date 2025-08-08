namespace FarmToTableData.Models
{
    public class PendingAnalysisDTO
    {
        public string InstanceId { get; set; }
        public int SentinelId { get; set; }
        public EEventType EventType { get; set; }
        public string JsonData { get; set; }
    }
}
