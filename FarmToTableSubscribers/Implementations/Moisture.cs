using FarmToTableData.Models;
using FarmToTableSubscribers.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FarmToTableSubscribers.Implementations
{
    public static class Moisture
    {
        [Function(nameof(PrepareMoistureAnalysis))]
        public static async Task PrepareMoistureAnalysis([ActivityTrigger] FunctionContext executionContext, dynamic input, WebAppClient webAppClient)
        {
            ChangeBase change = input.Change;
            string instanceId = input.InstanceId;

            ILogger logger = executionContext.GetLogger(nameof(PrepareMoistureAnalysis));
            logger.LogInformation($"Analyzing Moisture for SentinelId: {change.SentinelId} with InstanceId: {instanceId}...");

            try
            {
                await webAppClient.SavePendingAnalysis(instanceId, change.SentinelId, EEventType.Moisture);
            } catch (Exception ex)
            {
                logger.LogError($"Web app save failed: {ex.Message}.");
                throw; // durable function retries
            }
        }
    }
}
