using Journey_of_faith.Application.common.dtos.Events;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries
{
    public class GetEventCategoriesQuery : IRequest<IEnumerable<EventCategoryDto>>
    {
    }

    public class GetEventCategoriesHandler : IRequestHandler<GetEventCategoriesQuery, IEnumerable<EventCategoryDto>>
    {
        private readonly IEventQueries _eventQueries;

        public GetEventCategoriesHandler(IEventQueries eventQueries)
        {
            _eventQueries = eventQueries;
        }

        public async Task<IEnumerable<EventCategoryDto>> Handle(GetEventCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _eventQueries.GetCategoriesAsync(cancellationToken);
        }
    }
}
