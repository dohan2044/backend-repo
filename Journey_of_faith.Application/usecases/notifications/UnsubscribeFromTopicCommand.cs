using FirebaseAdmin.Messaging;
using Journey_of_faith.Application.common.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.notifications;

public class UnsubscribeFromTopicCommand : IRequest<TopicManagementResponse>
{
    public List<string> DeviceTokens { get; set; } = [];
    public string Topic { get; set; } = string.Empty;
}

public class UnsubscribeFromTopicHandler : IRequestHandler<UnsubscribeFromTopicCommand, TopicManagementResponse>
{
    private readonly IFirebaseNotification notification;

    public UnsubscribeFromTopicHandler(IFirebaseNotification notification) => this.notification = notification;

    public Task<TopicManagementResponse> Handle(UnsubscribeFromTopicCommand command, CancellationToken cancellationToken)
        => notification.UnsubscribeFromTopicAsync(command.DeviceTokens, command.Topic);
}
