namespace FarmToTableData.Models
{
    public class TemperatureReadingHistoryChange : HistoryChangeBase
    {
        private int TemperatureReadingHistoryId { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public override int HistoryId => TemperatureReadingHistoryId;
    }
}
