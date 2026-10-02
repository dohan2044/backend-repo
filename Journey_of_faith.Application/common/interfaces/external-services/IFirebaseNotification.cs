using Journey_of_faith.Domain.entities.notifications;
using FirebaseAdmin.Messaging;
namespace Journey_of_faith.Application.common.interfaces;

public interface IFirebaseNotification
{
    Task<bool> DeviceTokenExistsAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
    Task FcmRegisterAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
    Task<string> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    Task<string> SendNotificationAsync(string deviceToken, Guid userId, string title, string body, Dictionary<string, string>? data = null);
    Task<string> SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null);

    Task<TopicManagementResponse> SubscribeToTopicAsync(List<string> deviceTokens, string topic);
    Task<TopicManagementResponse> UnsubscribeFromTopicAsync(List<string> deviceTokens, string topic);
}
