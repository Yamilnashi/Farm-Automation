using FarmToTableData.Models;
using System;

namespace FarmToTableData.Interfaces
{
    public interface IChangeBase
    {
        ECdcChangeType Operation { get; set; }
        int SentinelId { get; set; }
        DateTime SavedDate { get; set; }
        EEventType EventType { get; }
    }
}
