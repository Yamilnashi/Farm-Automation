using System;

namespace FarmToTableData.Models
{
    public class AnalysisBase
    {
        public int AnalysisId { get; set; }
        public string InstanceId { get; set; }
        public bool IsAnalyzed { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
