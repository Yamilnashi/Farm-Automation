using System;

namespace FarmToTableData.Models
{
    public class Analysis
    {
        public int SentinelId { get; set; }
        public string SoilInstanceId { get; set; } = string.Empty;
        public string TemperatureInstanceId { get; set; } = string.Empty;
        public string MoistureInstanceId { get; set; } = string.Empty;
        public string SentinelStatusInstanceId { get; set; } = string.Empty;
    }
}
