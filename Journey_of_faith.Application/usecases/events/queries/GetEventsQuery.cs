using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.dtos.Events;
using Journey_of_faith.Domain.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.queries
{
    public class GetEventsQuery : IRequest<PagedResult<EventViewDto>>
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? StartFrom { get; set; }
        public DateTime? StartTo { get; set; }
        public bool OnlyUpcoming { get; set; } = true;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
    {
        public GetEventsQueryValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThan(0).WithMessage("PageIndex phải lớn hơn 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize phải nằm trong khoảng 1-100.");

            RuleFor(x => x.CategoryId)
                .Must(categoryId => categoryId is null || categoryId > 0)
                .WithMessage("Mã danh mục sự kiện không hợp lệ.");

            RuleFor(x => x)
                .Must(x => x.StartFrom is null || x.StartTo is null || x.StartTo >= x.StartFrom)
                .WithMessage("Khoảng thời gian lọc sự kiện không hợp lệ.");
        }
    }

    public class GetEventsHandler : IRequestHandler<GetEventsQuery, PagedResult<EventViewDto>>
    {
        private readonly IEventQueries _eventQueries;

        public GetEventsHandler(IEventQueries eventQueries)
        {
            _eventQueries = eventQueries;
        }

        public async Task<PagedResult<EventViewDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var filter = new EventFilterDto
            {
                Keyword = request.Keyword,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return await _eventQueries.GetEventsAsync(filter, cancellationToken);
        }
    }
}
