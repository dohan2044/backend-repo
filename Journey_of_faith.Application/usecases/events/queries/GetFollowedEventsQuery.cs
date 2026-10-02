using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.common.dtos.Events;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries
{
    public class GetFollowedEventsQuery : IRequest<IEnumerable<EventViewDto>>
    {
        public DateTime? StartFrom { get; set; }
        public DateTime? StartTo { get; set; }
    }

    public class GetFollowedEventsQueryValidator : AbstractValidator<GetFollowedEventsQuery>
    {
        public GetFollowedEventsQueryValidator()
        {
            RuleFor(x => x)
                .Must(x => x.StartFrom is null || x.StartTo is null || x.StartTo >= x.StartFrom)
                .WithMessage("Khoảng thời gian lọc sự kiện không hợp lệ.");
        }
    }

    public class GetFollowedEventsHandler : IRequestHandler<GetFollowedEventsQuery, IEnumerable<EventViewDto>>
    {
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public GetFollowedEventsHandler(IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<EventViewDto>> Handle(GetFollowedEventsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            return await _eventQueries.GetFollowedEventsAsync(userId, request.StartFrom, request.StartTo, cancellationToken);
        }
    }
}
