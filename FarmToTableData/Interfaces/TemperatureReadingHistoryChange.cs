using FarmToTableData.Models;

namespace FarmToTableData.Interfaces
{
    public class TemperatureReadingHistoryChange : HistoryChangeBase
    {
        private int TemperatureReadingHistoryId { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public override int HistoryId => TemperatureReadingHistoryId;
    }
}
