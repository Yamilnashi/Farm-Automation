using FarmToTableData.Interfaces;
using System;

namespace FarmToTableData.Models
{
    public class TemperatureReadingHistoryChange : IChangeBase
    {
        public int TemperatureReadingHistoryId { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public EEventType EventType { get { return EEventType.Temperature; } }
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public TemperatureReadingHistoryChange() {}
    }
}
