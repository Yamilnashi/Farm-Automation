using Azure.Messaging.EventHubs;
using FarmToTableData.Extensions;
using FarmToTableData.Models;
using FarmToTableSubscribers.Implementations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FarmToTableSubscribers;
public static class FarmToTableOrchestration
{
    #region Fields
    private static ILogger? _logger;
    #endregion

    [Function(nameof(EventHubTrigger))]
    public static async Task EventHubTrigger(
        [EventHubTrigger("FarmToTableEvents", Connection = "EventHubConnectionString")] EventData[] events,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        _logger = executionContext.GetLogger(nameof(EventHubTrigger));
        await Task.Delay(1000);
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
            _logger!.LogError($"Could not get string value from context in Orchestrator: {ex.Message}");
            return;
        }        

        ActivityInput? prepInput = JsonConvert.DeserializeObject<ActivityInput>(activityString);
        if (prepInput != null &&
            prepInput.EventType != EEventType.Sentinel) // will handle this later
        {
            prepInput.InstanceId = context.InstanceId;
            string json = JsonConvert.SerializeObject(prepInput);
            await GetPrepareAnalysisActivity(prepInput.EventType, json, context);
            AnalysisResult result = await context.WaitForExternalEvent<AnalysisResult>(prepInput.EventType.EventName());
        }
        return;
    }

    [Function(nameof(RaiseAnalysisResult))]
    public static async Task<HttpResponseData> RaiseAnalysisResult(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "raise-event/{instanceId}/{eventName}")] HttpRequestData request,
        string instanceId,
        string eventName,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(RaiseAnalysisResult));
        logger.LogInformation($"Raising event '{eventName}' for instance '{instanceId}'.");

        string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        AnalysisResult? result = JsonConvert.DeserializeObject<AnalysisResult>(requestBody);
        if (result == null)
        {
            HttpResponseData badResponse = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid AnalysisResult payload.");
            return badResponse;
        }

        await client.RaiseEventAsync(instanceId, eventName, result);
        HttpResponseData response = request.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Eevent raised successfully.");
        return response;
    }

    #region Private   
    private static Task GetPrepareAnalysisActivity(EEventType type, string json, TaskOrchestrationContext context)
    {
        return type switch
        {
            EEventType.Moisture => context.CallActivityAsync(nameof(Moisture.PrepareMoistureAnalysis), json),
            EEventType.Temperature => context.CallActivityAsync(nameof(Temperature.PrepareTemperatureAnalysis), json),
            EEventType.Soil => context.CallActivityAsync(nameof(Soil.PrepareSoilAnalysis), json),
            EEventType.SentinelStatus => context.CallActivityAsync(nameof(Implementations.SentinelStatus.PrepareSentinelStatusAnalysis), json),
            _ => throw new NotImplementedException()
        };
    }

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