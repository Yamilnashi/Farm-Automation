using FarmToTableData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmToTableData.Interfaces
{
    public interface IDboDbContextRead
    {
        Task<HistoryState> HistoryStateGet();
        Task<IEnumerable<MoistureReadingHistoryChange>> MoistureReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        Task<IEnumerable<SentinelChange>> SentinelChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        Task<IEnumerable<Sentinel>> SentinelList();
        Task<IEnumerable<SentinelStatus>> SentinelStatusCodes();
        Task<IEnumerable<SentinelStatusHistoryChange>> SentinelStatusHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        Task<IEnumerable<SoilReadingHistoryChange>> SoilReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        Task<IEnumerable<TemperatureReadingHistoryChange>> TemperatureReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        Task<byte[]> CdcMaxLogSequenceNumberGet();
        Task<byte[]> CdcMinLogSequenceNumberGet(string tableName);
        Task<byte[]> CdcIncrementLastLogSequenceNumberGet(byte[] lastLogSequenceNumber);
        Task<byte[]> CdcMaxLogSequenceNumberGetPerTable(string tableName);
        Task<IEnumerable<Analysis>> AnalysisList(bool? isAnalyzed = false);
    }
}
