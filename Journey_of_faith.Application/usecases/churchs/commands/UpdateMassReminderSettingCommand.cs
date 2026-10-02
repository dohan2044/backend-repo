using FluentValidation;
using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class UpdateMassReminderSettingCommand : IRequest<ReminderSettingDto>
    {
        public bool IsEnabled { get; set; }
        public int MinutesBefore { get; set; } = 30;
        public string? SpeechGender { get; set; }
        public double? SpeechSpeed { get; set; }
    }

    public class UpdateMassReminderSettingCommandValidator : AbstractValidator<UpdateMassReminderSettingCommand>
    {
        public UpdateMassReminderSettingCommandValidator()
        {
            RuleFor(x => x.MinutesBefore)
                .InclusiveBetween(1, 180)
                .WithMessage("Thời gian nhắc trước phải nằm trong khoảng 1 đến 180 phút.");

            RuleFor(x => x.SpeechGender)
                .MaximumLength(50)
                .WithMessage("Giá trị giới tính giọng đọc không được vượt quá 50 ký tự.");

            RuleFor(x => x.SpeechSpeed)
                .Must(speed => speed is null || (speed >= 0.5 && speed <= 2.0))
                .WithMessage("Tốc độ đọc phải nằm trong khoảng 0.5 đến 2.0.");
        }
    }

    public class UpdateMassReminderSettingHandler : IRequestHandler<UpdateMassReminderSettingCommand, ReminderSettingDto>
    {
        private readonly IChurchRepository _churchRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMassReminderSettingHandler(IChurchRepository churchRepository, ICurrentUserService currentUserService)
        {
            _churchRepository = churchRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ReminderSettingDto> Handle(UpdateMassReminderSettingCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            var setting = await _churchRepository.SaveReminderSettingAsync(
                userId,
                request.IsEnabled,
                request.MinutesBefore,
                request.SpeechGender,
                request.SpeechSpeed
            );

            return new ReminderSettingDto
            {
                MassReminderEnabled = setting.MassReminderEnabled,
                MinutesBefore = setting.MinutesBefore,
                SpeechGender = setting.SpeechGender,
                SpeechSpeed = setting.SpeechSpeed
            };
        }
    }
}
