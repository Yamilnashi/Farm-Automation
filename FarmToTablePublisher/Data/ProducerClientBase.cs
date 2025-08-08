using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using FarmToTableData.Implementations;
using FarmToTableData.Models;
using System.Text.Json;
using FarmToTableData.Interfaces;

namespace FarmToTablePublisher.Data
{
    public abstract class ProducerClientBase<T> : IAsyncDisposable where T : class, IChangeBase
    {
        #region Fields
        protected abstract string TableName { get; }
        protected abstract string EventName { get; }
        protected readonly ProducerClientBase<T> _instance;        
        protected readonly DboDbContext _dbContext;
        protected readonly EventHubProducerClient _producerClient;
        protected EventDataBatch? _eventBatch;
        #endregion

        #region Constructor
        public ProducerClientBase()
        {            
            _dbContext = new DboDbContext(Program.DbConnectionString);
            _producerClient = new EventHubProducerClient(Program.EventHubConnectionString, Program.EventHubName);
            _eventBatch = null;
            _instance = this;
        }
        #endregion

        #region Abstracts
        protected abstract Task<IEnumerable<T>> GetCdcChangeList(byte[] fromLogSequenceNumber, byte[] toLogSequenceNumber);
        protected abstract Task<byte[]?> GetCdcLastSequenceNumber();
        protected abstract Task HistoryStateUpdate(byte[] logSequenceNumber);
        #endregion

        #region Methods
        public Task<byte[]?> GetCdcMaxLogSequenceNumberPerTable()
        {
            return _dbContext.CdcMaxLogSequenceNumberGetPerTable(TableName);
        }
        public Task<byte[]?> GetCdcFromLogSequenceNumber()
        {
            return _dbContext.CdcMinLogSequenceNumberGet(TableName);
        }
        public async Task<ProducerClientBase<T>> Initialize()
        {
            _eventBatch = await _producerClient.CreateBatchAsync();
            return _instance;
        }
        public async Task PublishChanges()
        {
            byte[]? lastLogSequenceNumber = await GetCdcLastSequenceNumber();
            byte[]? maxLogSequenceNumber = await GetCdcMaxLogSequenceNumberPerTable();
            if (maxLogSequenceNumber == null)
            {
                return; // no changes ever in db
            }

            byte[]? fromLogSequenceNumber = lastLogSequenceNumber == null
                ? await GetCdcFromLogSequenceNumber()
                : await _dbContext.CdcIncrementLastLogSequenceNumberGet(lastLogSequenceNumber);

            if (fromLogSequenceNumber == null ||
                CompareLogSequenceNumbers(fromLogSequenceNumber, maxLogSequenceNumber) >= 0)
            {
                return; // no new changes
            }

            IEnumerable<T> changeList = await GetCdcChangeList(fromLogSequenceNumber, maxLogSequenceNumber);            
            T[] changes = changeList.ToArray();

            int deleteCount = 0;
            int insertCount = 0;
            int updateCount = 0;

            for (int i = 0; i < changes.Length; i++)
            {
                T change = changes[i];
                if (change.Operation == ECdcChangeType.Delete)
                {
                    deleteCount++;
                }
                else if (change.Operation == ECdcChangeType.Insert)
                {
                    insertCount++;
                }
                else if (change.Operation == ECdcChangeType.UpdateAfter)
                {
                    updateCount++;
                }
                else
                {
                    Console.WriteLine($"Unknown Operation: {changes[i]}");
                }

                byte[] eventDataBytes = JsonSerializer.SerializeToUtf8Bytes(change);
                EventData eventData = new EventData(eventDataBytes);
                if (_eventBatch != null &&
                    _eventBatch.TryAdd(eventData))
                {
                    await HistoryStateUpdate(maxLogSequenceNumber);
                }
            }

            if (deleteCount + insertCount + updateCount > 0)
            {
                Console.WriteLine($"{EventName} | Deleted: {deleteCount}, New: {insertCount}, Update: {updateCount}");
            }

            await SendBatchAsync();
        }
        public async Task SendBatchAsync()
        {
            if (_eventBatch != null &&
                _eventBatch.Count > 0)
            {
                await _producerClient.SendAsync(_eventBatch);
                Console.WriteLine($"Published {_eventBatch.Count} events. | {EventName}");
                _eventBatch.Dispose();
                _eventBatch = null;
            }
        }
        #endregion

        #region Private
        private int CompareLogSequenceNumbers(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++) // log sequence numbrs are always byte[10]
            {
                if (a[i] != b[i])
                {
                    return a[i].CompareTo(b[i]);
                }                
            }
            return 0;
        }
        #endregion

        #region IAsyncDisposable
        public async ValueTask DisposeAsync()
        {
            if (_eventBatch != null)
            {
                _eventBatch.Dispose();
                _eventBatch = null;
            }

            await _producerClient.DisposeAsync();
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
