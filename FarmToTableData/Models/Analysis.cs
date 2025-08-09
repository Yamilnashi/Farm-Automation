using System;

namespace FarmToTableData.Models
{
    public class Analysis
    {
        public int AnalysisId { get; set; }
        public int SentinelId { get; set; }
        public string InstanceId { get; set; }
        public int? SoilAnalysisId { get; set; }
        public int? TemperatureAnalysisId { get; set; }
        public int? MoistureAnalysisId { get; set; }
        public int? SentinelStatusAnalysisId { get; set; }
        public int? NPpm { get; set; }
        public int? PPpm { get; set; }
        public int? KPpm { get; set; }       
        public bool IsAnalyzed { get; set; }
        public decimal? TemperatureCelsius { get; set; }
        public byte? Moisture { get; set; }
        public ESentinelStatus? SentinelStatusCode { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
