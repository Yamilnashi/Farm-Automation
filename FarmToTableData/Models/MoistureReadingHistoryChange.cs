namespace FarmToTableData.Models
{
    public class MoistureReadingHistoryChange : HistoryChangeBase
    {
        private int MoistureReadingHistoryId { get; set; }
        public byte Moisture { get; set; }
        public override int HistoryId => MoistureReadingHistoryId;
    }
}
