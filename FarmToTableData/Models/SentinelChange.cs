using FarmToTableData.Interfaces;
using System;

namespace FarmToTableData.Models
{
    public class SentinelChange : IChangeBase
    {
        public EEventType EventType { get { return EEventType.Sentinel; } }
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
    }
}
