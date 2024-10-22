using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace UrlSave.Application.Extensions;

public static class ServiceExtensions
{
    public static void AddHangfireInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions()
            {
                QueuePollInterval = TimeSpan.FromSeconds(10),
                TransactionTimeout = TimeSpan.FromMilliseconds(500),
                SchemaName = "hangfire",
                PrepareSchemaIfNecessary = true,
            }));

        services.AddHangfireServer();
    }
}