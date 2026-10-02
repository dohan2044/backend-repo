using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos.Events;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries
{
    public class GetEventDetailsQuery : IRequest<EventDetailsDto?>
    {
        public int EventId { get; set; }
    }

    public class GetEventDetailsQueryValidator : AbstractValidator<GetEventDetailsQuery>
    {
        public GetEventDetailsQueryValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Mã sự kiện không hợp lệ.");
        }
    }

    public class GetEventDetailsHandler : IRequestHandler<GetEventDetailsQuery, EventDetailsDto?>
    {
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public GetEventDetailsHandler(IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<EventDetailsDto?> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            return await _eventQueries.GetEventDetailsAsync(request.EventId, userId, cancellationToken);
        }
    }
}
