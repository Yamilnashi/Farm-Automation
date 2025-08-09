using Microsoft.Data.SqlClient;
using System;
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
        SqlConnection GetConnection();
        Task<int> AnalysisSave(int sentinelId, DateTime savedDate);
        Task MoistureAnalysisSave(int analysisId, string instanceId, byte moisture, DateTime savedDate);
        Task TemperatureAnalysisSave(int analysisId, string intanceId, decimal temperatureCelsius, DateTime savedDate);
        Task SoilAnalysisSave(int analysisId, string instanceId, int nPpm, int pPpm, int kPpm, DateTime savedDate);
        Task SentinelStatusAnalysisSave(int analysisId, string instanceId, int sentinelStatusCode, DateTime savedDate);
        Task AnalysisUpdate(int analysisId);
    }
}
