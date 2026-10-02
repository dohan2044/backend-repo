using FluentValidation;
using Journey_of_faith.Application.common.dtos.songs;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.queries;

public sealed class GetSongCategoryMappingsQuery : IRequest<IReadOnlyList<SongCategoryMappingDto>>
{
    public int? SongId { get; set; }
    public int? CategoryId { get; set; }
}

public sealed class GetSongCategoryMappingsQueryValidator : AbstractValidator<GetSongCategoryMappingsQuery>
{
    public GetSongCategoryMappingsQueryValidator()
    {
        RuleFor(x => x.SongId).GreaterThan(0).When(x => x.SongId.HasValue);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
    }
}

public sealed class GetSongCategoryMappingsHandler
    : IRequestHandler<GetSongCategoryMappingsQuery, IReadOnlyList<SongCategoryMappingDto>>
{
    private readonly ISongQueries _queries;

    public GetSongCategoryMappingsHandler(ISongQueries queries) => _queries = queries;

    public Task<IReadOnlyList<SongCategoryMappingDto>> Handle(
        GetSongCategoryMappingsQuery request, CancellationToken cancellationToken)
        => _queries.GetSongCategoryMappingsAsync(request.SongId, request.CategoryId, cancellationToken);
}
