using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands
{
    public class CreateEventCategoryCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CreateEventCategoryCommandValidator : AbstractValidator<CreateEventCategoryCommand>
    {
        public CreateEventCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên danh mục sự kiện không được để trống.")
                .MaximumLength(200).WithMessage("Tên danh mục sự kiện không được vượt quá 200 ký tự.");
        }
    }

    public class CreateEventCategoryHandler : IRequestHandler<CreateEventCategoryCommand, int>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public CreateEventCategoryHandler(IEventRepository eventRepository, IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateEventCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out _))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            if (!string.Equals(_currentUserService.GetRoleUserName, "admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("Bạn không có quyền tạo danh mục sự kiện.");
            }

            if (await _eventQueries.CategoryNameExistsAsync(request.Name.Trim(), cancellationToken))
            {
                throw new ConfictException("Tên danh mục sự kiện đã tồn tại.");
            }

            return await _eventRepository.CreateCategoryAsync(request.Name.Trim());
        }
    }
}
