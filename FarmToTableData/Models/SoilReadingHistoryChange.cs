namespace FarmToTableData.Models
{
    public class SoilReadingHistoryChange : HistoryChangeBase
    {
        private int SoilReadingHistoryId { get; set; }
        public int NPpm { get; set; }
        public int PPpm { get; set; }
        public int KPpm { get; set; }
        public override int HistoryId => SoilReadingHistoryId;
    }
}
