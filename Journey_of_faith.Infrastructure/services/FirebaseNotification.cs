using FirebaseAdmin.Messaging;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace Journey_of_faith.Infrastructure.services;


public class FirebaseNotification : IFirebaseNotification
{
    private readonly ApplicationDbContext _dbContext;
    public FirebaseNotification(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<string> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
    {
        var message = new Message
        {
            Token = deviceToken,
            Notification = new Notification { Title = title, Body = body },
            Data = data
        };

        return FirebaseMessaging.DefaultInstance.SendAsync(message);
    }

    public async Task<string> SendNotificationAsync(string deviceToken,Guid userId, string title, string body, Dictionary<string, string>? data = null)
    {
        var message = new Message()
        {
            Token = deviceToken, // token thiết bị nhận thông báo
            Notification = new Notification() // tiêu đề và nội dung của thông báo
            {
                Title = title, 
                Body = body
            },
            Data = data
        };
        var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        await _dbContext.NotificationLogs.AddAsync(new Domain.entities.events.NotificationLogs
        {
            Title = title,
            Body = body,
            UserId = userId,
            SendAt = DateTime.UtcNow,
            TargetTopic = null
        });
        await _dbContext.SaveChangesAsync();
        return result;
    }
    public async Task<string> SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null)
    {
        var message = new Message()
        {
            Topic = topic,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Data = data
        };

        var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        await _dbContext.NotificationLogs.AddAsync(new Domain.entities.events.NotificationLogs
        {
            Title = title,
            Body = body,
            SendAt = DateTime.UtcNow,
            TargetTopic = topic
        });
        await _dbContext.SaveChangesAsync();
        return result;
         
    }

    public async Task FcmRegisterAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.DeviceTokens.AddAsync(deviceToken, cancellationToken);
    }

    public async Task<bool> DeviceTokenExistsAsync(DeviceToken deviceToken, CancellationToken cancellationToken  = default)
    {
        return await _dbContext.DeviceTokens.AnyAsync(
            e => e.UserId == deviceToken.UserId && 
                e.Token != deviceToken.Token && 
                e.Platform == deviceToken.Platform,
            cancellationToken
        );
    }

    /// <summary>
    /// Đăng ký danh sách Token thiết bị vào một Topic cụ thể.
    /// </summary>
    public async Task<TopicManagementResponse> SubscribeToTopicAsync(List<string> deviceTokens, string topic)
    {
        if (deviceTokens == null || !deviceTokens.Any())
        {
            throw new ArgumentException("Danh sách Device Tokens không được để trống.");
        }

        // Firebase hỗ trợ đăng ký tối đa 1000 tokens trong một lần gọi API
        return await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(deviceTokens, topic);
    }

    /// <summary>
    /// Xóa danh sách Token thiết bị khỏi một Topic cụ thể.
    /// </summary>
    public async Task<TopicManagementResponse> UnsubscribeFromTopicAsync(List<string> deviceTokens, string topic)
    {
        if (deviceTokens == null || !deviceTokens.Any())
        {
            throw new ArgumentException("Danh sách Device Tokens không được để trống.");
        }

        return await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(deviceTokens, topic);
    }
}
