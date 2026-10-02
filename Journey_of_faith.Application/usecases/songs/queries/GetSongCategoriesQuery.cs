using Journey_of_faith.Application.common.dtos.songs;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.queries;

public sealed record GetSongCategoriesQuery : IRequest<IReadOnlyList<SongCategoryDto>>;

public sealed class GetSongCategoriesHandler : IRequestHandler<GetSongCategoriesQuery, IReadOnlyList<SongCategoryDto>>
{
    private readonly ISongQueries _queries;

    public GetSongCategoriesHandler(ISongQueries queries) => _queries = queries;

    public Task<IReadOnlyList<SongCategoryDto>> Handle(GetSongCategoriesQuery request, CancellationToken cancellationToken)
        => _queries.GetSongCategoriesAsync(cancellationToken);
}
