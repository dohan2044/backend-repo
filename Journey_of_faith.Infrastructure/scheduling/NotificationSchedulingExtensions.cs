using Journey_of_faith.Application.common.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Journey_of_faith.Infrastructure.common;
using Quartz;

namespace Journey_of_faith.Infrastructure.scheduling;

public static class NotificationSchedulingExtensions
{
    public static IServiceCollection AddNotificationScheduling(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.AddLoggedScoped<INotificationScheduler, QuartzNotificationScheduler>();

        services.AddQuartz(quartz =>
        {
            quartz.SchedulerName = "JourneyOfFaithNotifications";
            quartz.SchedulerId = "AUTO";
            quartz.UseDefaultThreadPool(pool => pool.MaxConcurrency = 5);

            if (configuration.GetValue("NotificationScheduling:UsePersistentStore", true))
            {
                // đọc cấu hình quartz từ database
                var connectionString = configuration.GetConnectionString("Quartz")
                    ?? configuration.GetConnectionString("Connection");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("A SQL Server connection string is required for Quartz.");

                quartz.UsePersistentStore(store =>
                {
                    // store.ProvisionSchema(); tạo bảng vào trong db nếu chưa tồn tại
                    // đảm bảo JsonMapData lưu dưới dạng key-value string
                    store.UseProperties = true;
                    // khi app khởi động quartz kiểm tra xem có đúng Schema mong đợi hay không
                    store.PerformSchemaValidation = true;
                    // Cấu hình kết nối sql server
                    store.UseSqlServer(sql =>
                    {
                        sql.ConnectionString = connectionString; 
                        sql.TablePrefix = "dbo.QRTZ_";
                    });
                    store.UseSystemTextJsonSerializer(); // Serializer Json cho JobDataMap()
                    store.UseClustering(); // cho phép nhiều instance app cùng chạy xong xong, cùng trỏ vào 1 db
                });
            }
            else
            {
                // Explicit opt-in for local development/tests. Schedules are lost on restart.
                quartz.UseInMemoryStore();
            }
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        return services;
    }
}
