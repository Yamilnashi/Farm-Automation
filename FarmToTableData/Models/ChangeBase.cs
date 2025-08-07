using System;

namespace FarmToTableData.Models
{
    public class ChangeBase
    {
        public ECdcChangeType Operation { get; set; }
        public int SentinelId { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
