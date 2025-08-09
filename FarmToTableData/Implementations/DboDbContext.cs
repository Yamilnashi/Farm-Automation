using Dapper;
using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FarmToTableData.Implementations
{
    public class DboDbContext : IDisposable, IDboDbContextRead, IDboDbContextWrite
    {
        #region Fields
        private readonly SqlConnection _connection;
        #endregion

        #region Constructor
        public DboDbContext(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.OpenAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _connection.Dispose();
        }

        public SqlConnection GetConnection()
        {
            return _connection;
        }

        public Task<HistoryState> HistoryStateGet()
        {
            string sql = "[dbo].[HistoryStateGet]";
            return _connection.QueryFirstOrDefaultAsync<HistoryState>(sql, commandType: CommandType.StoredProcedure);
        }

        public Task HistoryStateUpdate(byte[] lastTemperatureReadingLsn = null,
            byte[] lastMoistureReadingLsn = null,
            byte[] lastSoilReadingLsn = null,
            byte[] lastStatusLsn = null,
            byte[] lastSentinelLsn = null)
        {
            string sql = "[dbo].[HistoryStateUpdate]";
            var values = new { lastTemperatureReadingLsn, lastMoistureReadingLsn, lastSoilReadingLsn, lastStatusLsn, lastSentinelLsn };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task<int> MoistureReadingHistoryAdd(int sentinelId, byte moisture)
        {
            string sql = "[dbo].[MoistureReadingHistoryAdd]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@sentinelId", sentinelId);
            p.Add("@moisture", moisture);
            p.Add("@MoistureReadingHistoryId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
            return Task.Run(() => p.Get<int>("MoistureReadingHistoryId"));
        }

        public Task<IEnumerable<MoistureReadingHistoryChange>> MoistureReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            string sql = "[dbo].[MoistureReadingHistoryChangeList]";
            var values = new { fromLogSequenceNumber, toLogSequenceNumber };
            return _connection.QueryAsync<MoistureReadingHistoryChange>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task ResetSentinel()
        {
            string sql = "[dbo].[ResetSentinel]";
            return _connection.ExecuteAsync(sql, commandType: CommandType.StoredProcedure);
        }

        public Task<int> SentinelAdd(float positionX, float positionY)
        {
            string sql = "[dbo].[SentinelAdd]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@positionX", positionX);
            p.Add("@positionY", positionY);
            p.Add("@SentinelId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
            return Task.Run(() => p.Get<int>("SentinelId"));
        }

        public Task<IEnumerable<SentinelChange>> SentinelChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            string sql = "[dbo].[SentinelChangeList]";
            var values = new { fromLogSequenceNumber, toLogSequenceNumber };
            return _connection.QueryAsync<SentinelChange>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<Sentinel>> SentinelList()
        {
            string sql = "[dbo].[SentinelList]";
            return _connection.QueryAsync<Sentinel>(sql, commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<SentinelStatus>> SentinelStatusCodes()
        {
            string sql = "[dbo].[SentinelStatusCodes]";
            return _connection.QueryAsync<SentinelStatus>(sql, commandType: CommandType.StoredProcedure);
        }

        public Task<int> SentinelStatusHistoryAdd(int sentinelId, int sentinelStatusCode)
        {
            string sql = "[dbo].[SentinelStatusHistoryAdd]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@sentinelId", sentinelId);
            p.Add("@sentinelStatusCode", sentinelStatusCode);
            p.Add("@SentinelStatusHistoryId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
            return Task.Run(() => p.Get<int>("SentinelStatusHistoryId"));
        }

        public Task<IEnumerable<SentinelStatusHistoryChange>> SentinelStatusHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            string sql = "[dbo].[SentinelStatusHistoryChangeList]";
            var values = new { fromLogSequenceNumber, toLogSequenceNumber };
            return _connection.QueryAsync<SentinelStatusHistoryChange>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task<int> SoilReadingHistoryAdd(int sentinelId, int nPpm, int pPpm, int kPpm)
        {
            string sql = "[dbo].[SoilReadingHistoryAdd]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@sentinelId", sentinelId);
            p.Add("@nPpm", nPpm);
            p.Add("@pPpm", pPpm);
            p.Add("@kPpm", kPpm);
            p.Add("@SoilReadingHistoryId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
            return Task.Run(() => p.Get<int>("SoilReadingHistoryId"));
        }

        public Task<IEnumerable<SoilReadingHistoryChange>> SoilReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            string sql = "[dbo].[SoilReadingHistoryChangeList]";
            var values = new { fromLogSequenceNumber, toLogSequenceNumber };
            return _connection.QueryAsync<SoilReadingHistoryChange>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task<int> TemperatureReadingHistoryAdd(int sentinelId, decimal temperatureCelsius)
        {
            string sql = "[dbo].[TemperatureReadingHistoryAdd]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@sentinelId", sentinelId);
            p.Add("@temperatureCelsius", temperatureCelsius);
            p.Add("@TemperatureReadingHistoryId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
            return Task.Run(() => p.Get<int>("TemperatureReadingHistoryId"));
        }

        public Task<IEnumerable<TemperatureReadingHistoryChange>> TemperatureReadingHistoryChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            string sql = "[dbo].[TemperatureReadingHistoryChangeList]";
            var values = new { fromLogSequenceNumber, toLogSequenceNumber };
            return _connection.QueryAsync<TemperatureReadingHistoryChange>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task<byte[]> CdcMaxLogSequenceNumberGet()
        {
            string sql = "select sys.fn_cdc_get_max_lsn()";
            return _connection.ExecuteScalarAsync<byte[]>(sql, commandType: CommandType.Text);
        }

        public Task<byte[]> CdcMinLogSequenceNumberGet(string tableName)
        {
            string sql = "select sys.fn_cdc_get_min_lsn(@tableName)";
            var values = new { tableName };
            return _connection.ExecuteScalarAsync<byte[]>(sql, values, commandType: CommandType.Text);
        }

        public Task<byte[]> CdcMaxLogSequenceNumberGetPerTable(string tableName)
        {
            string sql = $"select max(__$start_lsn) from cdc.[{tableName}_CT]";
            return _connection.ExecuteScalarAsync<byte[]>(sql, commandType: CommandType.Text);
        }

        public Task<byte[]> CdcIncrementLastLogSequenceNumberGet(byte[] lastLogSequenceNumber)
        {
            string sql = "select sys.fn_cdc_increment_lsn(@lastLogSequenceNumber)";
            var values = new { lastLogSequenceNumber };
            return _connection.ExecuteScalarAsync<byte[]>(sql, values, commandType: CommandType.Text);
        }

        public Task<int> AnalysisSave(int sentinelId, DateTime savedDate)
        {
            string sql = "[dbo].[AnalysisSave]";
            DynamicParameters p = new DynamicParameters();
            p.Add("@sentinelId", sentinelId);
            p.Add("@savedDate", savedDate);
            p.Add("@AnalysisId", dbType: DbType.Int32, direction: ParameterDirection.Output);
           
            return Task.Run(async () =>
            {
                await _connection.ExecuteAsync(sql, p, commandType: CommandType.StoredProcedure);
                return p.Get<int>("AnalysisId");
            });
        }

        public Task MoistureAnalysisSave(int analysisId, string instanceId, byte moisture, DateTime savedDate)
        {
            string sql = "[dbo].[MoistureAnalysisSave]";
            var values = new { analysisId, instanceId, moisture, savedDate };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task TemperatureAnalysisSave(int analysisId, string instanceId, decimal temperatureCelsius, DateTime savedDate)
        {
            string sql = "[dbo].[TemperatureAnalysisSave]";
            var values = new { analysisId, instanceId, temperatureCelsius, savedDate };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task SoilAnalysisSave(int analysisId, string instanceId, int nPpm, int pPpm, int kPpm, DateTime savedDate)
        {
            string sql = "[dbo].[SoilAnalysisSave]";
            var values = new { analysisId, instanceId, nPpm, pPpm, kPpm, savedDate };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task SentinelStatusAnalysisSave(int analysisId, string instanceId, int sentinelStatusCode, DateTime savedDate)
        {
            string sql = "[dbo].[SentinelStatusAnalysisSave]";
            var values = new { analysisId, instanceId, sentinelStatusCode, savedDate };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        public async Task<(IEnumerable<AnalysisBase>, IEnumerable<SoilAnalysis>, IEnumerable<MoistureAnalysis>, IEnumerable<TemperatureAnalysis>, IEnumerable<SentinelStatusAnalysis>)> AnalysisList(bool? isAnalyzed = false)
        {
            string sql = "[dbo].[AnalysisList]";
            var values = new { isAnalyzed };
            SqlMapper.GridReader multi = await _connection.QueryMultipleAsync(sql, values, commandType: CommandType.StoredProcedure);
            IEnumerable<AnalysisBase> analyses = await multi.ReadAsync<AnalysisBase>();
            IEnumerable<SoilAnalysis> soils = await multi.ReadAsync<SoilAnalysis>();
            IEnumerable<MoistureAnalysis> moistures = await multi.ReadAsync<MoistureAnalysis>();
            IEnumerable<TemperatureAnalysis> temperatures = await multi.ReadAsync<TemperatureAnalysis>();
            IEnumerable<SentinelStatusAnalysis> statuses = await multi.ReadAsync<SentinelStatusAnalysis>();
            return (analyses, soils, moistures, temperatures, statuses);
        }

        public Task<Analysis> AnalysisGet(int analysisId)
        {
            string sql = "[dbo].[AnalysisGet]";
            var values = new { analysisId };
            return _connection.QueryFirstOrDefaultAsync<Analysis>(sql, values, commandType: CommandType.StoredProcedure);
        }

        public Task AnalysisUpdate(int analysisId)
        {
            string sql = "[dbo].[AnalysisUpdate]";
            var values = new { analysisId };
            return _connection.ExecuteAsync(sql, values, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}
