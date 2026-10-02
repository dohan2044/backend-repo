using System.Text.Json;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Infrastructure.scheduling;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Quartz;

namespace UnitTesting.Scheduling;

public sealed class FirebaseNotificationJobTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Execute_SendsSerializedPayloadToCorrectFirebaseTarget(bool useDeviceToken)
    {
        var request = NewRequest(useDeviceToken);
        var notification = new Mock<IFirebaseNotification>(MockBehavior.Strict);
        if (useDeviceToken)
        {
            notification.Setup(service => service.SendNotificationAsync(
                    "test-device-token", "Prayer reminder", "It is time to pray.",
                    It.Is<Dictionary<string, string>?>(data => data != null && data["articleId"] == "123")))
                .ReturnsAsync("firebase-message-id");
        }
        else
        {
            notification.Setup(service => service.SendToTopicAsync(
                    "faith-reminders", "Prayer reminder", "It is time to pray.",
                    It.Is<Dictionary<string, string>?>(data => data != null && data["articleId"] == "123")))
                .ReturnsAsync("firebase-message-id");
        }
        var job = new FirebaseNotificationJob(notification.Object, NullLogger<FirebaseNotificationJob>.Instance);

        await job.Execute(NewContext(request));

        notification.VerifyAll();
        notification.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Execute_FirebaseFailureDoesNotImmediatelyRefire()
    {
        var request = NewRequest(useDeviceToken: false);
        var notification = new Mock<IFirebaseNotification>(MockBehavior.Strict);
        var sendFailure = new InvalidOperationException("Simulated Firebase outage");
        notification.Setup(service => service.SendToTopicAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>?>()))
            .ThrowsAsync(sendFailure);
        var job = new FirebaseNotificationJob(notification.Object, NullLogger<FirebaseNotificationJob>.Instance);

        var failure = await Assert.ThrowsAsync<JobExecutionException>(() => job.Execute(NewContext(request)));

        Assert.False(failure.RefireImmediately);
        Assert.Same(sendFailure, failure.InnerException);
        notification.Verify(service => service.SendToTopicAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>?>()), Times.Once);
        notification.VerifyNoOtherCalls();
    }

    private static ScheduleNotificationRequest NewRequest(bool useDeviceToken) => new()
    {
        Title = "Prayer reminder",
        Body = "It is time to pray.",
        Token = useDeviceToken ? "test-device-token" : null,
        Topic = useDeviceToken ? null : "faith-reminders",
        Data = new Dictionary<string, string> { ["articleId"] = "123" },
        RunAt = new DateTimeOffset(2030, 1, 1, 1, 0, 0, TimeSpan.Zero)
    };

    private static IJobExecutionContext NewContext(ScheduleNotificationRequest request)
    {
        var detail = JobBuilder.Create<FirebaseNotificationJob>()
            .WithIdentity($"test-notification-{Guid.NewGuid():N}")
            .UsingJobData(FirebaseNotificationJob.PayloadKey, JsonSerializer.Serialize(request))
            .Build();
        var context = new Mock<IJobExecutionContext>();
        context.SetupGet(value => value.JobDetail).Returns(detail);
        context.SetupGet(value => value.MergedJobDataMap).Returns(detail.JobDataMap);
        context.SetupGet(value => value.CancellationToken).Returns(CancellationToken.None);
        return context.Object;
    }
}
