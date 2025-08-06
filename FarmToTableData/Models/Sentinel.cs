using System;

namespace FarmToTableData.Models
{
    public class Sentinel
    {
        public int SentinelId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public ESentinelStatus SentinelStatusCode { get; set; }
        public string SentinelStatusName { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
