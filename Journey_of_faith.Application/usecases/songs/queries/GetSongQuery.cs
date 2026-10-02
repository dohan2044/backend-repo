using FluentValidation;
using Journey_of_faith.Application.common.dtos.songs;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.queries;

public sealed record GetSongQuery(int Id) : IRequest<SongDto?>;

public sealed class GetSongQueryValidator : AbstractValidator<GetSongQuery>
{
    public GetSongQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public sealed class GetSongHandler : IRequestHandler<GetSongQuery, SongDto?>
{
    private readonly ISongQueries _queries;

    public GetSongHandler(ISongQueries queries) => _queries = queries;

    public Task<SongDto?> Handle(GetSongQuery request, CancellationToken cancellationToken)
        => _queries.GetSongAsync(request.Id, cancellationToken);
}
