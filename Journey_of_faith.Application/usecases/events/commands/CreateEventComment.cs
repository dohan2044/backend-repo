using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.events;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands;

public class CreateEventComment : IRequest<bool>
{
    public int EventId {get; set;}
    public string Comment {get; set;}
    public DateTime Created {get; set;} = DateTime.Now;
}


public class CreateEventCommentHandler :  IRequestHandler<CreateEventComment, bool>
{
    private readonly IEventRepository eventRepository;
    private readonly ICurrentUserService currentUserService;
    public CreateEventCommentHandler(IEventRepository eventRepository, ICurrentUserService currentUserService)
    {
        this.eventRepository = eventRepository;
        this.currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CreateEventComment comment, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out Guid userId))
        {
            throw new UnauthorizationException("Người dùng không hợp lệ");
        } 
        var eventComment = new EventComment
        {
            UserId = userId,
            EventId = comment.EventId,
            Comment = comment.Comment,
            CreatedTime = comment.Created
        };

        return await eventRepository.CreateEventCommentAsync(eventComment);
    }
}