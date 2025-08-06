using FarmToTableData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmToTableData.Interfaces
{
    public interface IDboDbContextRead
    {
        Task<HistoryState> HistoryStateGet();
        Task<IEnumerable<MoistureReadingHistoryChange>> MoistureReadingHistoryChangeList();
        Task<IEnumerable<SentinelChange>> SentinelChangeList();
        Task<IEnumerable<Sentinel>> SentinelList();
        Task<IEnumerable<SentinelStatus>> SentinelStatusCodes();
        Task<IEnumerable<SentinelStatusHistoryChange>> SentinelStatusHistoryChangeList();
        Task<IEnumerable<SoilReadingHistoryChange>> SoilReadingHistoryChangeList();
        Task<IEnumerable<TemperatureReadingHistoryChange>> TemperatureReadingHistoryChangeList();

    }
}
