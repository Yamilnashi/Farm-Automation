using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using FarmToTableData.Models;
using System.Text;
using System.Text.Json;

namespace FarmToTableFieldOps
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Endpoint=sb://farmtotablenamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iIjZvoaAQM9HV04pK+5NqFid4ASAWWgPt+AEhOmrY2Q=";
            string eventHubName = "FarmToTableEvents";
            var testChange = new
            {
                SentinelId = 1,
                Operation = ECdcChangeType.Insert
            };
            string jsonPayload = JsonSerializer.Serialize(testChange);
            await using var producer = new EventHubProducerClient(connectionString, eventHubName);
            using EventDataBatch eventBatch = await producer.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonPayload)));
            await producer.SendAsync(eventBatch);
            Console.WriteLine("Test event sent—check Function logs/breakpoints.");
        }
    }
}
