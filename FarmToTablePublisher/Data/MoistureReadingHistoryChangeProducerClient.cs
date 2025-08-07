using FarmToTableData.Models;

namespace FarmToTablePublisher.Data
{
    public class MoistureReadingHistoryChangeProducerClient : ProducerClientBase<MoistureReadingHistoryChange>
    {
        #region Fields
        protected override string TableName => "dbo_MoistureReadingHistory";
        protected override string EventName => "Moisture Reading";
        #endregion

        #region Methods
        protected override Task<IEnumerable<MoistureReadingHistoryChange>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber)
        {
            return _dbContext.MoistureReadingHistoryChangeList(fromLogSequenceNumber, toLogSequenceNumber);
        }
        protected override Task<byte[]?> GetCdcLastSequenceNumber()
        {
            return Task.Run(async () =>
            {
                HistoryState state = await _dbContext.HistoryStateGet();
                return state.LastMoistureReadingLsn ?? null;
            });
        }
        protected override Task HistoryStateUpdate(byte[] logSequenceNumber)
        {
            return _dbContext.HistoryStateUpdate(lastMoistureReadingLsn: logSequenceNumber);
        }
        #endregion
    }
}
