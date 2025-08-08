using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTablePublisher.Data;
using KeyVaultAccessLib;
using Microsoft.Extensions.Configuration;

namespace FarmToTablePublisher
{
    internal class Program
    {
        #region Fields
        private const int _cdcPollingIntervalSeconds = 1; // every 1 second, we don't want to hammer the db
        public static string DbConnectionString = string.Empty;
        public static string EventHubConnectionString = string.Empty;
        public static string EventHubName = string.Empty;
        #endregion

        #region Main
        static async Task Main(string[] args)
        {
            IConfigurationRoot? _ = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationRoot config = _!;

            (DbConnectionString, EventHubConnectionString, EventHubName) = await BuildResourceConnections(config);

            // poll CDC tables and publish loop
            while (true)
            {
                List<Task> tasks = new List<Task>()
                {
                    CreatePublishTask(() => new SentinelChangeProducerClient()),
                    CreatePublishTask(() => new SentinelStatusHistoryChangeProducerClient()),
                    CreatePublishTask(() => new TemperatureReadingHistoryChangeProducerClient()),
                    CreatePublishTask(() => new MoistureReadingHistoryChangeProducerClient()),
                    CreatePublishTask(() => new SoilReadingHistoryChangeProducerClient())
                };

                await Task.WhenAll(tasks);
                await Task.Delay(1000 * _cdcPollingIntervalSeconds);
            }
        }
        #endregion

        #region Private
        private static async Task<(string, string, string)> BuildResourceConnections(IConfigurationRoot config)
        {
            string? vaultUrl = config["KeyVaultUrl"];
            string? sqlServer = config["SqlServer"];
            string? eventHubConnString = config["EventHubConnectionString"];
            string? eventHub = config["EventHubName"];

            if (string.IsNullOrWhiteSpace(vaultUrl) ||
                string.IsNullOrEmpty(sqlServer) ||
                string.IsNullOrEmpty(eventHubConnString) ||
                string.IsNullOrEmpty(eventHub))
            {
                throw new NullReferenceException("Check your appsettings.json file there could be a missing setting.");
            }

            KeyVaultClient? kvClient = new KeyVaultClient(vaultUrl);
            if (kvClient == null)
            {
                throw new NullReferenceException("Could not initialize the KeyVaultClient.");
            }

            string sqlUsername = await kvClient.GetSecretAsync("SqlUsername");
            string sqlPassword = await kvClient.GetSecretAsync("SqlPassword");

            string dbConnString = $"Server={sqlServer};" +
                $"Database=FarmToTable;" +
                $"User Id={sqlUsername};" +
                $"Password={sqlPassword};" +
                $"TrustServerCertificate=True;";

            return (dbConnString, eventHubConnString, eventHub);
        }
        private static Task CreatePublishTask<T>(Func<ProducerClientBase<T>> clientFactory) 
            where T : class, IChangeBase
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<T> client = await clientFactory().Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }        
        #endregion
    }
}
