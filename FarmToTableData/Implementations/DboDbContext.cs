using Dapper;
using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FarmToTableData.Implementations
{
    public class DboDbContext : IAsyncDisposable, IDisposable, IDboDbContextRead, IDboDbContextWrite
    {
        #region Fields
        private readonly ILogger<DboDbContext> _logger;
        private readonly SqlConnection _connection;
        #endregion

        #region Constructor
        public DboDbContext(ILogger<DboDbContext> logger, string connectionString)
        {
            _logger = logger;
            _connection = new SqlConnection(connectionString);
            _connection.OpenAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _connection.Dispose();
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync();
        }

        public Task<HistoryState> HistoryStateGet()
        {
            string sql = "[dbo].[HistoryStateGet]";
            return _connection.QueryFirstOrDefaultAsync<HistoryState>(sql, commandType: CommandType.StoredProcedure);
        }

        public Task HistoryStateUpdate(byte[] lastTemperatureReadingLsn = null,
            byte[] lastMoistureReadingLsn = null,
            byte[] lastSoilReadingLsn = null,
            byte[] lastStatusLsn = null)
        {
            string sql = "[dbo].[HistoryStateUpdate]";
            var values = new { lastTemperatureReadingLsn, lastMoistureReadingLsn, lastSoilReadingLsn, lastStatusLsn };
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

        public Task<IEnumerable<MoistureReadingHistoryChange>> MoistureReadingHistoryChangeList()
        {
            string sql = "[dbo].[MoistureReadingHistoryChangeList]";
            return _connection.QueryAsync<MoistureReadingHistoryChange>(sql, commandType: CommandType.StoredProcedure);
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

        public Task<IEnumerable<SentinelChange>> SentinelChangeList()
        {
            string sql = "[dbo].[SentinelChangeList]";
            return _connection.QueryAsync<SentinelChange>(sql, commandType: CommandType.StoredProcedure);
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

        public Task<IEnumerable<SentinelStatusHistoryChange>> SentinelStatusHistoryChangeList()
        {
            string sql = "[dbo].[SentinelStatusHistoryChangeList]";
            return _connection.QueryAsync<SentinelStatusHistoryChange>(sql, commandType: CommandType.StoredProcedure);
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

        public Task<IEnumerable<SoilReadingHistoryChange>> SoilReadingHistoryChangeList()
        {
            string sql = "[dbo].[SoilReadingHistoryChaangeList]";
            return _connection.QueryAsync<SoilReadingHistoryChange>(sql, commandType: CommandType.StoredProcedure);
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

        public Task<IEnumerable<TemperatureReadingHistoryChange>> TemperatureReadingHistoryChangeList()
        {
            string sql = "[dbo].[TemperatureReadingHistoryChangeList]";
            return _connection.QueryAsync<TemperatureReadingHistoryChange>(sql, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
