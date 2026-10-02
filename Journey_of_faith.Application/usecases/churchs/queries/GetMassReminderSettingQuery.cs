using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.queries
{
    public class GetMassReminderSettingQuery : IRequest<ReminderSettingDto>
    {
    }

    public class GetMassReminderSettingHandler : IRequestHandler<GetMassReminderSettingQuery, ReminderSettingDto>
    {
        private readonly IChurchQueries _churchQueries;
        private readonly ICurrentUserService _currentUserService;

        public GetMassReminderSettingHandler(IChurchQueries churchQueries, ICurrentUserService currentUserService)
        {
            _churchQueries = churchQueries;
            _currentUserService = currentUserService;
        }

        public async Task<ReminderSettingDto> Handle(GetMassReminderSettingQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Không xác định được người dùng hiện tại.");
            }

            return await _churchQueries.GetReminderSettingAsync(userId, cancellationToken);
        }
    }
}
