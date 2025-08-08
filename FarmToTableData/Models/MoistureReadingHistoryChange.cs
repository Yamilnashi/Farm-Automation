using FarmToTableData.Interfaces;
using System;

namespace FarmToTableData.Models
{
    public class MoistureReadingHistoryChange : IChangeBase
    {
        public EEventType EventType { get { return EEventType.Moisture; } }
        public int MoistureReadingHistoryId { get; set; }
        public byte Moisture { get; set; }
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public MoistureReadingHistoryChange() { }
    }
}
