using System.Threading.Tasks;

namespace FarmToTableData.Interfaces
{
    public interface IDboDbContextWrite
    {
        Task<int> SentinelAdd(float positionX, float positionY);
        Task HistoryStateUpdate(byte[] lastTemperatureReadingLsn = null, 
            byte[] lastMoistureReadingLsn = null, 
            byte[] lastSoilReadingLsn = null, 
            byte[] lastStatusLsn = null,
            byte[] lastSentinelLsn = null);
        Task<int> MoistureReadingHistoryAdd(int sentinelId, byte moisture);
        Task<int> TemperatureReadingHistoryAdd(int sentinelId, decimal temperatureCelsius);
        Task<int> SoilReadingHistoryAdd(int sentinelId, int nPpm, int pPpm, int kPpm);
        Task ResetSentinel();
        Task<int> SentinelStatusHistoryAdd(int sentinelId, int sentinelStatusCode);
            
    }
}
