using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands
{
    public class FollowEventCommand : IRequest<bool>
    {
        public int EventId { get; set; }
    }

    public class UnfollowEventCommand : IRequest<bool>
    {
        public int EventId { get; set; }
    }

    public class FollowEventCommandValidator : AbstractValidator<FollowEventCommand>
    {
        public FollowEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Mã sự kiện không hợp lệ.");
        }
    }

    public class UnfollowEventCommandValidator : AbstractValidator<UnfollowEventCommand>
    {
        public UnfollowEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Mã sự kiện không hợp lệ.");
        }
    }

    public class FollowEventHandler : IRequestHandler<FollowEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public FollowEventHandler(IEventRepository eventRepository, IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(FollowEventCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            if (!await _eventQueries.EventExistsAsync(request.EventId, cancellationToken))
            {
                throw new NotFoundException("Không tìm thấy sự kiện.");
            }

            if (await _eventQueries.IsFollowingEventAsync(userId, request.EventId, cancellationToken))
            {
                throw new ConfictException("Bạn đã theo dõi sự kiện này.");
            }

            return await _eventRepository.FollowEventAsync(userId, request.EventId);
        }
    }

    public class UnfollowEventHandler : IRequestHandler<UnfollowEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public UnfollowEventHandler(IEventRepository eventRepository, IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UnfollowEventCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            if (!await _eventQueries.EventExistsAsync(request.EventId, cancellationToken))
            {
                throw new NotFoundException("Không tìm thấy sự kiện.");
            }

            if (!await _eventQueries.IsFollowingEventAsync(userId, request.EventId, cancellationToken))
            {
                throw new NotFoundException("Bạn chưa theo dõi sự kiện này.");
            }

            return await _eventRepository.UnfollowEventAsync(userId, request.EventId);
        }
    }
}
