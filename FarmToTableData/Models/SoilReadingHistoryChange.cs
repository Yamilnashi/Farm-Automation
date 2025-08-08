using FarmToTableData.Interfaces;
using System;

namespace FarmToTableData.Models
{
    public class SoilReadingHistoryChange : IChangeBase
    {
        public int SoilReadingHistoryId { get; set; }
        public int NPpm { get; set; }
        public int PPpm { get; set; }
        public int KPpm { get; set; }
        public EEventType EventType { get { return EEventType.Soil; } }
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public SoilReadingHistoryChange() { }
    }
}
