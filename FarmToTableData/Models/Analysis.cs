using System;

namespace FarmToTableData.Models
{
    public class Analysis
    {
        public int AnalysisId { get; set; }
        public int SentinelId { get; set; }
        public string InstanceId { get; set; }
        public bool IsAnalyzed { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
