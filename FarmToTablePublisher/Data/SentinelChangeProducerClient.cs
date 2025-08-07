using FarmToTableData.Models;

namespace FarmToTablePublisher.Data
{
    public class SentinelChangeProducerClient : ProducerClientBase<SentinelChange>
    {
        #region Fields
        protected override string TableName => "dbo_Sentinel";
        protected override string EventName => "Sentinel Changes";
        #endregion

        #region Methods
        protected override Task<IEnumerable<SentinelChange>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            return _dbContext.SentinelChangeList(fromLogSequenceNumber, toLogSequenceNumber);
        }
        protected override Task<byte[]?> GetCdcLastSequenceNumber()
        {
            return Task.Run(async () =>
            {
                HistoryState state = await _dbContext.HistoryStateGet();
                return state.LastSentinelLsn ?? null;
            });
        }
        protected override Task HistoryStateUpdate(byte[] logSequenceNumber)
        {
            return _dbContext.HistoryStateUpdate(lastSentinelLsn: logSequenceNumber);
        }
        #endregion
    }
}
