using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystem.Service.BackgroundJobs
{
    public static class HangfireJobsExtension
    {
        public static IServiceCollection ConfigureHangfireJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), 
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount * 2;
                options.Queues = new[] { "default", "low-stock-checks", "archiving" };
            });

            return services;
        }

        public static void ScheduleRecurringJobs()
        {
            RecurringJob.AddOrUpdate<InventoryJobs>(
                "check-low-stock",
                job => job.CheckLowStockProductsAsync(),
                Cron.Daily(0, 0),
                TimeZoneInfo.Local,
                "low-stock-checks");

            RecurringJob.AddOrUpdate<InventoryJobs>(
                "archive-old-transactions",
                job => job.ArchiveOldTransactionsAsync(365),
                Cron.Weekly(DayOfWeek.Sunday, 1),
                TimeZoneInfo.Local,
                "archiving");
        }
    }
}
