using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.churchs;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class FollowChurchCommand : IRequest<bool>
    {
        public int ChurchId { get; set; }
    }
    public class FollowChurchCommandValidator : AbstractValidator<FollowChurchCommand>
    {
        public FollowChurchCommandValidator()
        {
            RuleFor(x => x.ChurchId)
                .GreaterThan(0)
                .WithMessage("Mã nhà thờ không hợp lệ.");
        }
    }
    public class FollowChurchHandler : IRequestHandler<FollowChurchCommand, bool>
    {
        private readonly IChurchRepository _churchRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChurchQueries _churchQueries;
        private readonly IUnitOfWork _unitOfWork;
        public FollowChurchHandler(
            IChurchRepository churchRepository,
            ICurrentUserService currentUserService,
            IChurchQueries churchQueries,
            IUnitOfWork unitOfWork)
        {
            _churchRepository = churchRepository;
            _currentUserService = currentUserService;
            _churchQueries = churchQueries;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(FollowChurchCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            if (!await _churchQueries.ChurchExistsAsync(request.ChurchId, cancellationToken))
            {
                throw new NotFoundException("Không tìm thấy nhà thờ.");
            }

            if (await _churchQueries.IsFollowingChurchAsync(userId, request.ChurchId, cancellationToken))
            {
                throw new ConfictException("Nhà thờ đã nằm trong danh sách theo dõi.");
            }

            await _churchRepository.FollowChurchAsync(
                new UserChurch { UserId = userId, ChurchId = request.ChurchId },
                cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
            return true;
        }
    }
}
