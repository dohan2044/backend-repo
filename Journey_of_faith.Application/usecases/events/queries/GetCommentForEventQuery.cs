using Journey_of_faith.Application.common.dtos.Events;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries;

public class GetCommentForEventQuery : IRequest<IReadOnlyList<EventCommentDto>>
{
    public int EventId {get; set;}
}

public class GetCommentForEventHandler : IRequestHandler<GetCommentForEventQuery, IReadOnlyList<EventCommentDto>>
{
    private readonly IEventQueries eventQueries;
    public GetCommentForEventHandler(IEventQueries eventQueries)
    {
        this.eventQueries = eventQueries;
    }

    public async Task<IReadOnlyList<EventCommentDto>> Handle(GetCommentForEventQuery query, CancellationToken cancellationToken = default)
    {
        return await eventQueries.GetCommentsAsync(query.EventId, cancellationToken);
    }
}
