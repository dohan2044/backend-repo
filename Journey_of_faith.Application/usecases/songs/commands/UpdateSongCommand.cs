using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public sealed class UpdateSongCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public int? AlbumId { get; set; }
    public int Duration { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? Lyric { get; set; }
    public int PlayCount { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UpdateSongCommandValidator : AbstractValidator<UpdateSongCommand>
{
    public UpdateSongCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ArtistId).GreaterThan(0);
        RuleFor(x => x.AlbumId).GreaterThan(0).When(x => x.AlbumId.HasValue);
        RuleFor(x => x.Duration).GreaterThan(0);
        RuleFor(x => x.AudioUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CoverImageUrl).MaximumLength(500);
        RuleFor(x => x.PlayCount).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateSongHandler : IRequestHandler<UpdateSongCommand, bool>
{
    private readonly ISongRepository _repository;
    private readonly ISongQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public UpdateSongHandler(ISongRepository repository, ISongQueries queries, ICurrentUserService currentUser)
    {
        _repository = repository;
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateSongCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");
        if (!await _queries.SongExistsAsync(request.Id, cancellationToken))
            throw new NotFoundException("Bài hát không tồn tại.");
        if (!await _queries.ArtistExistsAsync(request.ArtistId, cancellationToken))
            throw new NotFoundException("Nghệ sĩ không tồn tại.");
        if (request.AlbumId.HasValue && !await _queries.AlbumExistsAsync(request.AlbumId.Value, cancellationToken))
            throw new NotFoundException("Album không tồn tại.");

        var song = new Song(request.Title.Trim(), request.ArtistId, request.AlbumId, request.Duration,
            request.AudioUrl, request.CoverImageUrl, request.Lyric ?? string.Empty, request.PlayCount, request.IsActive);
        return await _repository.UpdateSongAsync(request.Id, song, userId, cancellationToken);
    }
}
