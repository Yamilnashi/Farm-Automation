using FarmToTableData.Implementations;
using FarmToTableData.Interfaces;
using FarmToTableWebApp.Hubs;
using KeyVaultAccessLib;

namespace FarmToTableWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfigurationRoot? _ = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationRoot config = _!;

            string connectionString = await BuildDbConnectionString(config);

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IDboDbContextWrite>(sp => new DboDbContext(connectionString));
            builder.Services.AddScoped<IDboDbContextRead>(sp => new DboDbContext(connectionString));
            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
            app.MapHub<AnalysisHub>("/analysisHub");
            await app.RunAsync();
        }

        private static async Task<string> BuildDbConnectionString(IConfigurationRoot config)
        {
            string? vaultUrl = config["KeyVaultUrl"];
            string? sqlServer = config["SqlServer"];

            if (string.IsNullOrWhiteSpace(vaultUrl) ||
                string.IsNullOrEmpty(sqlServer))
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

            return $"Server={sqlServer};" +
                $"Database=FarmToTable;" +
                $"User Id={sqlUsername};" +
                $"Password={sqlPassword};" +
                $"TrustServerCertificate=True;";
        }
    }
}
