using FirebaseAdmin.Messaging;
using Journey_of_faith.Application.common.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.notifications;

public class SubscribeToTopicCommand : IRequest<TopicManagementResponse>
{
    public List<string> DeviceTokens { get; set; } = [];
    public string Topic { get; set; } = string.Empty;
}

public class SubscribeToTopicHandler : IRequestHandler<SubscribeToTopicCommand, TopicManagementResponse>
{
    private readonly IFirebaseNotification notification;

    public SubscribeToTopicHandler(IFirebaseNotification notification) => this.notification = notification;

    public Task<TopicManagementResponse> Handle(SubscribeToTopicCommand command, CancellationToken cancellationToken)
        => notification.SubscribeToTopicAsync(command.DeviceTokens, command.Topic);
}
