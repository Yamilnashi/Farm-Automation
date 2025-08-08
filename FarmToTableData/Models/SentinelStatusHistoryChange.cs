using FarmToTableData.Interfaces;
using System;

namespace FarmToTableData.Models
{
    public class SentinelStatusHistoryChange : IChangeBase
    {
        public EEventType EventType { get { return EEventType.SentinelStatus; } }
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public ESentinelStatus SentinelStatusCode { get; set; }
        public int SentinelStatusHistoryId { get; set; }
        public SentinelStatusHistoryChange() { }
    }
}
