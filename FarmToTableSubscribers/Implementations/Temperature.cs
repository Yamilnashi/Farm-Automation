using FarmToTableData.Models;
using FarmToTableData.Utils;
using FarmToTableSubscribers.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace FarmToTableSubscribers.Implementations
{
    public class Temperature
    {
        #region Fields
        private readonly WebAppClient _httpClient;
        #endregion

        #region Constructor
        public Temperature(WebAppClient client)
        {
            _httpClient = client;
        }
        #endregion

        #region ActivityFunction
        [Function(nameof(PrepareTemperatureAnalysis))]
        public async Task PrepareTemperatureAnalysis([ActivityTrigger] string input, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(PrepareTemperatureAnalysis));
            if (string.IsNullOrEmpty(input))
            {
                logger.LogError("Input or Change is null-check orchestrator call.");
                return;
            }

            ActivityInput? activityInput = JsonConvert.DeserializeObject<ActivityInput>(input);
            if (activityInput == null)
            {
                logger.LogError($"Input had value but deserializing into ActivityInput failed.");
                return;
            }

            TemperatureReadingHistoryChange change = new CdcHelper(activityInput.Change)
                .GetTemperatureReadingHistoryChange();
            string jsonChange = JsonConvert.SerializeObject(change);
            logger.LogInformation($"Analyzing Temperature for SentinelId: {change.SentinelId} with InstanceId: {activityInput.InstanceId}...");

            try
            {
                await _httpClient.PutPendingAnalysis(activityInput.InstanceId, change.SentinelId, change.EventType, change.SavedDate, jsonChange);
            }
            catch (Exception ex)
            {
                logger.LogError($"Web app save failed: {ex.Message}.");
                throw; // durable function retries
            }
        }
        #endregion
    }
}
