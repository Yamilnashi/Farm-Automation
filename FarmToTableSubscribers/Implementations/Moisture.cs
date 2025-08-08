using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableData.Utils;
using FarmToTableSubscribers.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FarmToTableSubscribers.Implementations
{
    public class Moisture
    {
        #region Fields
        private readonly WebAppClient _httpClient;
        #endregion

        #region Constructor
        public Moisture(WebAppClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region ActivityFunction
        [Function(nameof(PrepareMoistureAnalysis))]
        public async Task PrepareMoistureAnalysis(
            [ActivityTrigger] string input,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(PrepareMoistureAnalysis));
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

            MoistureReadingHistoryChange change = CdcHelper.GetMoistureReadingHistoryChange(activityInput.Change);
            logger.LogInformation($"Analyzing Moisture for SentinelId: {change.SentinelId} with InstanceId: {activityInput.InstanceId}...");

            try
            {
                await _httpClient.PutPendingAnalysis(activityInput.InstanceId, change.SentinelId, EEventType.Moisture, input);
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
