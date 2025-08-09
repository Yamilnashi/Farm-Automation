using FarmToTableData.Models;
using FarmToTableData.Utils;
using FarmToTableSubscribers.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FarmToTableSubscribers.Implementations
{
    public class SentinelStatus
    {
        #region Fields
        private readonly WebAppClient _httpClient;
        #endregion

        #region Constructor
        public SentinelStatus(WebAppClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region  Methods
        [Function(nameof(PrepareSentinelStatusAnalysis))]
        public async Task PrepareSentinelStatusAnalysis([ActivityTrigger] string input, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(PrepareSentinelStatusAnalysis));
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

            SentinelStatusHistoryChange change = new CdcHelper(activityInput.Change)
                .GetSentinelStatusHistoryChange();
            string jsonChange = JsonConvert.SerializeObject(change);
            logger.LogInformation($"Analyzing Sentinel Status for SentinelId: {change.SentinelId} with InstanceId: {activityInput.InstanceId}...");

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
