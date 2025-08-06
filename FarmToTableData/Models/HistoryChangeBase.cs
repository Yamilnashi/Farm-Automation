using System;

namespace FarmToTableData.Models
{
    public abstract class HistoryChangeBase
    {
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
        public abstract int HistoryId { get; }

    }
}
