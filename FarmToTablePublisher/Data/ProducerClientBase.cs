using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using FarmToTableData.Implementations;
using System.Text.Json;

namespace FarmToTablePublisher.Data
{
    public abstract class ProducerClientBase<T> : IAsyncDisposable where T : class
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
        public ProducerClientBase(string connectionString, 
            string eventHubConnectionString, string eventHubName)
        {            
            _dbContext = new DboDbContext(connectionString);
            _producerClient = new EventHubProducerClient(eventHubConnectionString, eventHubName);
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
                Console.WriteLine($"{EventName}: There haven't been any changes at all.");
                return; // no changes ever in db
            }

            byte[]? fromLogSequenceNumber = lastLogSequenceNumber == null
                ? await GetCdcFromLogSequenceNumber()
                : await _dbContext.CdcIncrementLastLogSequenceNumberGet(lastLogSequenceNumber);

            if (fromLogSequenceNumber == null ||
                CompareLogSequenceNumbers(fromLogSequenceNumber, maxLogSequenceNumber) >= 0)
            {
                //Console.WriteLine($"{EventName}: Published 0 events.  No new changes.");
                return; // no new changes
            }

            IEnumerable<T> changeList = await GetCdcChangeList(fromLogSequenceNumber, maxLogSequenceNumber);
            T[] changes = changeList.ToArray();
            for (int i = 0; i < changes.Length; i++)
            {
                T change = changes[i];
                byte[] eventDataBytes = JsonSerializer.SerializeToUtf8Bytes(change);
                EventData eventData = new EventData(eventDataBytes);
                if (_eventBatch != null &&
                    _eventBatch.TryAdd(eventData))
                {
                    await HistoryStateUpdate(maxLogSequenceNumber);
                }
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
