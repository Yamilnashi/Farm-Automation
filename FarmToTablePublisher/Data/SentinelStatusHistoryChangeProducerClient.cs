using FarmToTableData.Models;

namespace FarmToTablePublisher.Data
{
    public class SentinelStatusHistoryChangeProducerClient : ProducerClientBase<SentinelStatusHistoryChange>
    {
        #region Fields
        protected override string TableName => "dbo_SentinelStatusHistory";
        protected override string EventName => "Sentinel Status History";
        #endregion

        #region Constructor
        public SentinelStatusHistoryChangeProducerClient(string connectionString, string eventHubConnectionString, string eventHubName)
            : base(connectionString, eventHubConnectionString, eventHubName) { }
        #endregion

        #region Methods
        protected override Task<IEnumerable<SentinelStatusHistoryChange>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            return _dbContext.SentinelStatusHistoryChangeList(fromLogSequenceNumber, toLogSequenceNumber);
        }
        protected override Task HistoryStateUpdate(byte[] logSequenceNumber)
        {
            return _dbContext.HistoryStateUpdate(lastStatusLsn: logSequenceNumber);
        }      
        protected override Task<byte[]?> GetCdcLastSequenceNumber()
        {
            return Task.Run(async () =>
            {
                HistoryState state = await _dbContext.HistoryStateGet();
                return state.LastStatusLsn ?? null;
            });
        }
        #endregion
    }
}
