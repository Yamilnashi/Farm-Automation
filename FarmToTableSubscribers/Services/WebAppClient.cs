using FarmToTableData.Models;
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
        public async Task SavePendingAnalysis(string instanceId, int sentinelId, EEventType type)
        {
            string apiEndpoint = "api/v1/pending-analysis";
            var payload = new
            {
                InstanceId = instanceId,
                SentinelId = sentinelId,
                Type = type
            };

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, payload);
            response.EnsureSuccessStatusCode();
        }
        #endregion
    }
}
