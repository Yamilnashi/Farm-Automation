namespace FarmToTableData.Models
{
    public class AnalysisResult
    {
        public string InstanceId { get; set; }
        public EEventType EventType { get; set; }
        public dynamic Data { get; set; }
    }
}
