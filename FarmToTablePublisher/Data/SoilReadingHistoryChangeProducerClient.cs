using FarmToTableData.Models;

namespace FarmToTablePublisher.Data
{
    public class SoilReadingHistoryChangeProducerClient : ProducerClientBase<SoilReadingHistoryChange>
    {
        #region Fields
        protected override string TableName => "dbo_SoilReadingHistory";
        protected override string EventName => "Soil Reading";
        #endregion

        #region Constructor
        public SoilReadingHistoryChangeProducerClient(string connectionString, string eventHubConnectionString, string eventHubName)
            : base(connectionString, eventHubConnectionString, eventHubName) { }
        #endregion

        #region Methods
        protected override Task<IEnumerable<SoilReadingHistoryChange>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            return _dbContext.SoilReadingHistoryChangeList(fromLogSequenceNumber, toLogSequenceNumber);
        }
        protected override Task<byte[]?> GetCdcLastSequenceNumber()
        {
            return Task.Run(async () =>
            {
                HistoryState state = await _dbContext.HistoryStateGet();
                return state.LastSoilReadingLsn ?? null;
            });
        }
        protected override Task HistoryStateUpdate(byte[] logSequenceNumber)
        {
            return _dbContext.HistoryStateUpdate(lastSoilReadingLsn: logSequenceNumber);
        }
        #endregion
    }
}
