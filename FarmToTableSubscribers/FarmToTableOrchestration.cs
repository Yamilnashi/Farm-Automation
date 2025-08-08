using Azure.Messaging.EventHubs;
using FarmToTableData.Extensions;
using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableSubscribers.Implementations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.ObjectiveC;

namespace FarmToTableSubscribers;
public static class FarmToTableOrchestration
{
    #region Fields
    private static ILogger? _logger;
    private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };
    #endregion

    [Function(nameof(EventHubTrigger))]
    public static async Task EventHubTrigger(
        [EventHubTrigger("FarmToTableEvents", Connection = "EventHubConnectionString")] EventData[] events,
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
        string? activityString;
        try
        {
            activityString = context.GetInput<string?>();
            if (string.IsNullOrEmpty(activityString))
            {
                return;
            }
        } catch (JsonException ex)
        {
            return;
        }        

        ActivityInput? prepInput = JsonConvert.DeserializeObject<ActivityInput>(activityString);

        if (prepInput != null)
        {
            prepInput.InstanceId = context.InstanceId;
            List<Task> prepTasks = new List<Task>();
            string preppedInputJsonString = JsonConvert.SerializeObject(prepInput);
            if (prepInput.EventType == EEventType.Moisture)
            {
                prepTasks.Add(context.CallActivityAsync(nameof(Moisture.PrepareMoistureAnalysis), preppedInputJsonString));
            }

            await Task.WhenAll(prepTasks);

            if (prepInput.EventType == EEventType.Moisture)
            {
                AnalysisResult moistureResult = await context.WaitForExternalEvent<AnalysisResult>(EEventType.Moisture.EventName());
            }
        }
    }

    #region Private   
    private static async Task ScheduleOrchestrator(DurableTaskClient client, EventData[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            dynamic? input;
            try
            {
                input = JsonConvert.DeserializeObject<dynamic>(events[i].EventBody.ToString());
            } catch (Exception)
            {
                continue; // cant deserialize, lets move on
            }
            
            if (input == null)
            {
                _logger!.LogInformation($"Found an empty change event, we should probably log this somewhere...");
                continue;
            }

            _logger!.LogInformation($"Received event: {input.Operation} for Sentinel: {input.SentinelId}");

            if (input is JObject jObj && 
                jObj.TryGetValue("EventType", out JToken? eventTypeToken) && 
                eventTypeToken != null)
            {
                ActivityInput activity = new ActivityInput()
                {
                    Change = jObj,
                    EventType = eventTypeToken.ToObject<EEventType>()
                };
                string activityString = JsonConvert.SerializeObject(activity);
                string? instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator), activityString);
            }

            
        }
    }
    #endregion

}