using FarmToTableData.Extensions;
using FarmToTableData.Models;
using Newtonsoft.Json;

namespace FarmToTableWebApp.Services
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
        public async Task RaiseAnalysisResult(AnalysisResult result)
        {

            string eventName = result.EventType.EventName();
            string apiEndpoint = $"raise-event/{result.InstanceId}/{eventName}";
            //string json = JsonConvert.SerializeObject(result);

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempt {attempt}: Posting to {apiEndpoint}");
                    HttpResponseMessage? response = await _httpClient.PostAsJsonAsync(apiEndpoint, result);
                    response.EnsureSuccessStatusCode();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(5000 * attempt); // exponential backoff
                }
            }
            throw new Exception($"Failed to raise event after max retries.");
        }
        #endregion
    }
}
