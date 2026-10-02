using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateSongHandler : IRequestHandler<CreateSongCommand, int>
{
    private readonly ISongRepository _repository;
    private readonly ISongQueries _queries;

    public CreateSongHandler(ISongRepository repository, ISongQueries queries)
    {
        _repository = repository;
        _queries = queries;
    }

    public async Task<int> Handle(CreateSongCommand command, CancellationToken cancellationToken)
    {
        if (!await _queries.ArtistExistsAsync(command.ArtistId, cancellationToken))
            throw new NotFoundException("Nghệ sĩ không tồn tại.");
        if (command.AlbumId.HasValue && !await _queries.AlbumExistsAsync(command.AlbumId.Value, cancellationToken))
            throw new NotFoundException("Album không tồn tại.");
        if (!await _queries.SongCategoryExistsAsync(command.CategorySongId, cancellationToken))
            throw new NotFoundException("Thể loại bài hát không tồn tại.");

        var song = new Song(command.Title.Trim(), command.ArtistId, command.AlbumId, command.Duration,
            command.AudioUrl, command.CoverImageUrl, command.Lyric ?? string.Empty, command.PlayCount, command.IsActive);
        return await _repository.CreateSongAsync(song, command.CategorySongId, cancellationToken);
    }
}
