using FarmToTableData.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace FarmToTableSubscribers.Services
{
    public class WebAppClient
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor
        public WebAppClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task PutPendingAnalysis(string instanceId, int sentinelId, EEventType type, string data)
        {
            string apiEndpoint = "api/v1/pending-analysis";
            var payload = new PendingAnalysisDTO()
            {
                InstanceId = instanceId,
                SentinelId = sentinelId,
                EventType = type,
                JsonData = data
            };
            string json = JsonConvert.SerializeObject(payload);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, json);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
