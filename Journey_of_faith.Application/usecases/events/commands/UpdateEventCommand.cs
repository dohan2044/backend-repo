using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands
{
    public class UpdateEventCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Mã sự kiện không hợp lệ.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề sự kiện không được để trống.")
                .MaximumLength(200).WithMessage("Tiêu đề sự kiện không được vượt quá 200 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Mô tả sự kiện không được vượt quá 2000 ký tự.");

            RuleFor(x => x.Location)
                .MaximumLength(255).WithMessage("Địa điểm không được vượt quá 255 ký tự.");

            RuleFor(x => x)
                .Must(x => x.EndDate is null || x.EndDate >= x.StartDate)
                .WithMessage("Thời gian kết thúc phải lớn hơn hoặc bằng thời gian bắt đầu.");

            RuleForEach(x => x.CategoryIds)
                .GreaterThan(0).WithMessage("Mã danh mục sự kiện không hợp lệ.");

            RuleForEach(x => x.ImageUrls)
                .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự.");
        }
    }

    public class UpdateEventHandler : IRequestHandler<UpdateEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventQueries _eventQueries;
        private readonly ICurrentUserService _currentUserService;

        public UpdateEventHandler(IEventRepository eventRepository, IEventQueries eventQueries, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _eventQueries = eventQueries;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            if (!await _eventQueries.EventExistsAsync(request.Id, cancellationToken))
            {
                throw new NotFoundException("Không tìm thấy sự kiện.");
            }

            if (request.CategoryIds is not null)
            {
                foreach (var categoryId in request.CategoryIds.Distinct())
                {
                    if (!await _eventQueries.CategoryExistsAsync(categoryId, cancellationToken))
                    {
                        throw new NotFoundException($"Không tìm thấy danh mục sự kiện với Id = {categoryId}.");
                    }
                }
            }

            var payload = new UpdateEventPayload
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ImageUrl = request.ImageUrl,
                LastModifierUserId = userId,
                CategoryIds = request.CategoryIds,
                ImageUrls = request.ImageUrls
            };

            return await _eventRepository.UpdateEventAsync(payload);
        }
    }
}
