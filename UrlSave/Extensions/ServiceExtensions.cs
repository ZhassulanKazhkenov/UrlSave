using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Mvc;

namespace UrlSave.Extensions;

public static class ServiceExtensions
{
    public static void AddApiVersioningExtension(this IServiceCollection services)
    {
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

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