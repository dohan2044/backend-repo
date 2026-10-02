using FluentValidation;
using Journey_of_faith.Application.common.dtos.songs;
using Journey_of_faith.Domain.dtos;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.queries;

public sealed class GetSongsQuery : IRequest<PagedResult<SongDto>>
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public sealed class GetSongsQueryValidator : AbstractValidator<GetSongsQuery>
{
    public GetSongsQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
    }
}

public sealed class GetSongsHandler : IRequestHandler<GetSongsQuery, PagedResult<SongDto>>
{
    private readonly ISongQueries _queries;

    public GetSongsHandler(ISongQueries queries) => _queries = queries;

    public Task<PagedResult<SongDto>> Handle(GetSongsQuery request, CancellationToken cancellationToken)
        => _queries.GetSongsAsync(new SongFilterDto
        {
            Keyword = request.Keyword,
            CategoryId = request.CategoryId,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        }, cancellationToken);
}
