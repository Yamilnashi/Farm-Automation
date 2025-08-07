using Azure.Messaging.EventHubs;
using FarmToTableData.Extensions;
using FarmToTableData.Models;
using FarmToTableSubscribers.Implementations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FarmToTableSubscribers;
public static class FarmToTableOrchestration
{
    #region Fields
    private static ILogger? _logger;
    #endregion
    public static async Task EventHubTrigger(
        [EventHubTrigger("FarmToTableEvents", Connection = "EventHubConnectioNString")] EventData[] events,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        _logger = executionContext.GetLogger(nameof(EventHubTrigger));
        if (_logger != null)
        {
            await ScheduleOrchestrator(client, events);
        }
    }

    [Function(nameof(Orchestrator))]
    public static async Task Orchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ChangeBase? input = context.GetInput<ChangeBase>();
        if (input != null)
        {
            var prepInput = new { context.InstanceId, Change = input };
            List<Task> prepTasks = new()
            {
                context.CallActivityAsync(nameof(Moisture.PrepareMoistureAnalysis), prepInput)
                // will add more later
            };

            await Task.WhenAll(prepTasks);

            AnalysisResult moistureResult = await context.WaitForExternalEvent<AnalysisResult>(EEventType.Moisture.EventName());
        }
    }

    #region Private   
    private static async Task ScheduleOrchestrator(DurableTaskClient client, EventData[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            ChangeBase? change = JsonSerializer.Deserialize<ChangeBase?>(events[i].EventBody.ToString());
            if (change == null)
            {
                _logger!.LogInformation($"Found an empty change event, we should probably log this somewhere...");
                continue;
            }

            _logger!.LogInformation($"Received event: {change.Operation} for Sentinel: {change.SentinelId}");
            string? instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator), change);
        }
    }
    #endregion

}