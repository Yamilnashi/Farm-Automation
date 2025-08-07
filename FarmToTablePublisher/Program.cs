using FarmToTableData.Models;
using FarmToTablePublisher.Data;
using KeyVaultAccessLib;
using Microsoft.Extensions.Configuration;

namespace FarmToTablePublisher
{
    internal class Program
    {
        #region Fields
        private const int _cdcPollingIntervalSeconds = 5; // every 5 seconds, we don't want to hammer the db
        private static string dbConnectionString = string.Empty;
        private static string eventHubConnectionString = string.Empty;
        private static string eventHubName = string.Empty;
        #endregion

        #region Main
        static async Task Main(string[] args)
        {
            IConfigurationRoot? _ = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationRoot config = _!;

            (dbConnectionString, eventHubConnectionString, eventHubName) = await BuildResourceConnections(config);

            // poll CDC tables and publish loop
            while (true)
            {
                List<Task> tasks = new List<Task>()
                {
                    SentinelChangeTask(),
                    SentinelStatusHistoryChangeTask(),
                    TemperatureReadingHistoryChangeTask(),
                    MoistureReadingHistoryChangeTask(),
                    SoilReadingHistoryChangeTask()
                };

                await Task.WhenAll(tasks);
                await Task.Delay(100 * _cdcPollingIntervalSeconds);
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
        private static Task SentinelChangeTask()
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<SentinelChange> client = await new SentinelChangeProducerClient(dbConnectionString, 
                    eventHubConnectionString, 
                    eventHubName).Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }
        private static Task SentinelStatusHistoryChangeTask()
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<SentinelStatusHistoryChange> client = await new SentinelStatusHistoryChangeProducerClient(dbConnectionString,
                    eventHubConnectionString, eventHubName).Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }
        private static Task TemperatureReadingHistoryChangeTask()
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<TemperatureReadingHistoryChange> client = await new TemperatureReadingHistoryChangeProducerClient(dbConnectionString,
                    eventHubConnectionString, eventHubName).Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }
        private static Task MoistureReadingHistoryChangeTask()
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<MoistureReadingHistoryChange> client = await new MoistureReadingHistoryChangeProducerClient(dbConnectionString,
                    eventHubConnectionString, eventHubName).Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }
        private static Task SoilReadingHistoryChangeTask()
        {
            return Task.Run(async () =>
            {
                await using (ProducerClientBase<SoilReadingHistoryChange> client = await new SoilReadingHistoryChangeProducerClient(dbConnectionString,
                    eventHubConnectionString, eventHubName).Initialize())
                {
                    await client.PublishChanges();
                }
            });
        }
        #endregion
    }
}
