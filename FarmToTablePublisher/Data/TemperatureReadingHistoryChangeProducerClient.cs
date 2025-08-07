using FarmToTableData.Models;

namespace FarmToTablePublisher.Data
{
    public class TemperatureReadingHistoryChangeProducerClient : ProducerClientBase<TemperatureReadingHistoryChange>
    {
        #region Fields
        protected override string TableName => "dbo_TemperatureReadingHistory";
        protected override string EventName => "Temperature Reading";
        #endregion

        #region Constructor
        public TemperatureReadingHistoryChangeProducerClient(string connectionString,
            string eventHubConnectionString, string eventHubName)
            : base(connectionString, eventHubConnectionString, eventHubName) { }
        #endregion

        #region Methods
        protected override Task<IEnumerable<TemperatureReadingHistoryChange>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            return _dbContext.TemperatureReadingHistoryChangeList(fromLogSequenceNumber, toLogSequenceNumber);
        }
        protected override Task<byte[]?> GetCdcLastSequenceNumber()
        {
            return Task.Run(async () =>
            {
                HistoryState state = await _dbContext.HistoryStateGet();
                return state.LastTemperatureReadingLsn ?? null;
            });
        }
        protected override Task HistoryStateUpdate(byte[] logSequenceNumber)
        {
            return _dbContext.HistoryStateUpdate(lastTemperatureReadingLsn: logSequenceNumber);
        }
        #endregion
    }
}
