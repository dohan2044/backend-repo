using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands
{
    public class DeleteEventCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
    {
        public DeleteEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Mã sự kiện không hợp lệ.");
        }
    }

    public class DeleteEventHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public DeleteEventHandler(IEventRepository eventRepository, IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }
            if (!await _eventQueries.EventExistsAsync(request.Id, cancellationToken))
            {
                throw new NotFoundException("Không tìm thấy sự kiện.");
            }

            return await _eventRepository.DeleteEventAsync(request.Id);
        }
    }
}
