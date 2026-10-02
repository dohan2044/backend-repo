// using FluentValidation;
// using Journey_of_faith.Application.common.interfaces;
// using Journey_of_faith.Application.exceptions;
// using Journey_of_faith.Application.usecases.churchs.dtos;
// using Journey_of_faith.Domain.interfaces;
// using MediatR;

// namespace Journey_of_faith.Application.usecases.churchs.queries
// {
//     public class GetPersonalizedMassSchedulesQuery : IRequest<IEnumerable<PersonalizedMassScheduleItemDto>>
//     {
//         public DateTime? FromDate { get; set; }
//         public DateTime? ToDate { get; set; }
//         public int? ChurchId { get; set; }
//     }

//     public class GetPersonalizedMassSchedulesQueryValidator : AbstractValidator<GetPersonalizedMassSchedulesQuery>
//     {
//         public GetPersonalizedMassSchedulesQueryValidator()
//         {
//             RuleFor(x => x.ChurchId)
//                 .Must(churchId => churchId is null || churchId > 0)
//                 .WithMessage("Mã nhà thờ không hợp lệ.");

//             RuleFor(x => x)
//                 .Must(x => x.FromDate is null || x.ToDate is null || x.ToDate.Value.Date >= x.FromDate.Value.Date)
//                 .WithMessage("Khoảng thời gian lọc lịch lễ không hợp lệ.");
//         }
//     }

//     public class GetPersonalizedMassSchedulesHandler
//         : IRequestHandler<GetPersonalizedMassSchedulesQuery, IEnumerable<PersonalizedMassScheduleItemDto>>
//     {
//         private readonly IChurchRepository _churchRepository;
//         private readonly ICurrentUserService _currentUserService;

//         public GetPersonalizedMassSchedulesHandler(IChurchRepository churchRepository, ICurrentUserService currentUserService)
//         {
//             _churchRepository = churchRepository;
//             _currentUserService = currentUserService;
//         }

//         public async Task<IEnumerable<PersonalizedMassScheduleItemDto>> Handle(GetPersonalizedMassSchedulesQuery request, CancellationToken cancellationToken)
//         {
//             if (!Guid.TryParse(_currentUserService.UserId, out var userId))
//             {
//                 throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
//             }

//             if (request.ChurchId.HasValue && !await _churchRepository.ChurchExistsAsync(request.ChurchId.Value))
//             {
//                 throw new NotFoundException("Không tìm thấy nhà thờ.");
//             }

//             var fromDate = (request.FromDate ?? DateTime.UtcNow.Date).Date;
//             var toDate = (request.ToDate ?? fromDate.AddDays(7)).Date;
//             if (toDate < fromDate)
//             {
//                 throw new BadRequestException("Khoảng thời gian lọc lịch lễ không hợp lệ.");
//             }

//             var schedules = await _churchRepository.GetPersonalizedMassSchedulesAsync(userId, fromDate, toDate, request.ChurchId);
//             var setting = await _churchRepository.GetReminderSettingAsync(userId);

//             return schedules.Select(schedule =>
//             {
//                 var massDate = schedule.Date?.Date ?? schedule.FromDate?.Date;
//                 DateTime? massStartAt = null;
//                 DateTime? reminderAt = null;

//                 if (massDate.HasValue)
//                 {
//                     massStartAt = massDate.Value.Add(schedule.Time);
//                     reminderAt = massStartAt.Value.AddMinutes(-setting.MinutesBefore);
//                 }

//                 return new PersonalizedMassScheduleItemDto
//                 {
//                     MassScheduleId = schedule.MassScheduleId,
//                     ChurchId = schedule.ChurchId,
//                     ChurchName = schedule.ChurchName,
//                     ChurchAddress = schedule.ChurchAddress,
//                     IsFixed = schedule.IsFixed,
//                     Date = schedule.Date,
//                     FromDate = schedule.FromDate,
//                     ToDate = schedule.ToDate,
//                     Time = schedule.Time,
//                     MassTypeId = schedule.MassTypeId,
//                     MassTypeName = schedule.MassTypeName,
//                     MassStartAt = massStartAt,
//                     IsReminderEnabled = setting.MassReminderEnabled,
//                     MinutesBefore = setting.MinutesBefore,
//                     ReminderAt = reminderAt
//                 };
//             }).ToList();
//         }
//     }
// }
