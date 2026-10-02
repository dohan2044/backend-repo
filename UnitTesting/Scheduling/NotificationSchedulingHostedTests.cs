using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Infrastructure.scheduling;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UnitTesting.Scheduling;

public sealed class NotificationSchedulingHostedTests
{
    [Fact]
    public async Task HostedScheduler_ActivatesScopedFirebaseJobAndExecutesOneOffOnce()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotificationScheduling:UsePersistentStore"] = "false"
            })
            .Build();
        var delivery = new RecordedDelivery();
        // An isolated host exercises real Quartz startup and DI, with no API, DB or Firebase app.
        using var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(delivery);
                services.AddScoped<IFirebaseNotification, RecordingFirebaseNotification>();
                services.AddNotificationScheduling(configuration);
            })
            .Build();
        await host.StartAsync();
        try
        {
            using var scope = host.Services.CreateScope();
            var scheduler = scope.ServiceProvider.GetRequiredService<INotificationScheduler>();
            var created = await scheduler.ScheduleAsync(new ScheduleNotificationRequest
            {
                Title = "Prayer reminder",
                Body = "It is time to pray.",
                Token = "test-device-token",
                Data = new Dictionary<string, string> { ["articleId"] = "123" },
                RunAt = DateTimeOffset.UtcNow.AddSeconds(2)
            });

            var received = await delivery.Completion.Task.WaitAsync(TimeSpan.FromSeconds(15));

            Assert.Equal("test-device-token", received.Target);
            Assert.Equal("Prayer reminder", received.Title);
            Assert.Equal("It is time to pray.", received.Body);
            Assert.Equal("123", received.Data!["articleId"]);

            // The delivery callback runs before Quartz finishes its cleanup. Wait for the
            // non-durable one-off job to disappear, proving there is no recurring trigger.
            using var cleanupTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            while (await scheduler.GetAsync(created.Id, cleanupTimeout.Token) is not null)
                await Task.Delay(25, cleanupTimeout.Token);

            Assert.Equal(1, delivery.Count);
        }
        finally
        {
            using var shutdownTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await host.StopAsync(shutdownTimeout.Token);
        }
    }

    private sealed record Message(string Target, string Title, string Body, Dictionary<string, string>? Data);

    private sealed class RecordedDelivery
    {
        public TaskCompletionSource<Message> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Count;
    }

    private sealed class RecordingFirebaseNotification(RecordedDelivery delivery) : IFirebaseNotification
    {
        public Task<bool> DeviceTokenExistsAsync(
            DeviceToken deviceToken, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("The scheduling job should not query registered device tokens.");

        public Task FcmRegisterAsync(
            DeviceToken deviceToken, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("The scheduling job should not register device tokens.");

        public Task<string> SendNotificationAsync(
            string deviceToken, string title, string body, Dictionary<string, string>? data = null)
        {
            Interlocked.Increment(ref delivery.Count);
            delivery.Completion.TrySetResult(new Message(deviceToken, title, body, data));
            return Task.FromResult("recorded-firebase-message-id");
        }

        public Task<string> SendNotificationAsync(
            string deviceToken, Guid userId, string title, string body, Dictionary<string, string>? data = null)
            => SendNotificationAsync(deviceToken, title, body, data);

        public Task<string> SendToTopicAsync(
            string topic, string title, string body, Dictionary<string, string>? data = null)
            => throw new InvalidOperationException("This one-off job should target a device token.");

        public Task<TopicManagementResponse> SubscribeToTopicAsync(List<string> deviceTokens, string topic)
            => throw new InvalidOperationException("The scheduling job should not subscribe tokens to a topic.");

        public Task<TopicManagementResponse> UnsubscribeFromTopicAsync(List<string> deviceTokens, string topic)
            => throw new InvalidOperationException("The scheduling job should not unsubscribe tokens from a topic.");
    }
}
