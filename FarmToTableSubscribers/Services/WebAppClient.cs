using FarmToTableData.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Net.Http.Json;

namespace FarmToTableSubscribers.Services
{
    public class WebAppClient
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private const int _maxRetries = 3;
        #endregion

        #region Constructor
        public WebAppClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Methods
        public async Task PutPendingAnalysis(string instanceId, int sentinelId, EEventType type, DateTime savedDate, string data)
        {;
            string apiEndpoint = "api/v1/pending-analysis";
            var payload = new PendingAnalysisDTO()
            {
                InstanceId = instanceId,
                SentinelId = sentinelId,
                EventType = type,
                SavedDate = savedDate,
                JsonData = data
            };
            string json = JsonConvert.SerializeObject(payload);

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempt {attempt}: Fetching {apiEndpoint}");
                    HttpResponseMessage? response = await _httpClient.PutAsJsonAsync(apiEndpoint, json);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(5000 * attempt); // exponential backoff
                }
            }
        }
        #endregion
    }
}
