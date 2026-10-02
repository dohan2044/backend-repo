using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateAlbumHandler : IRequestHandler<CreateAlbumCommand, int>
{
    private readonly ISongRepository songRepository;
    public CreateAlbumHandler(ISongRepository songRepository)
    {
        this.songRepository = songRepository;
    }

    public async Task<int> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
    {
        var album = new Album(command.Title, command.ArtistId, command.ReleaseYear, command.CoverImageUrl);
        return await songRepository.CreateAlbumAsync(album, cancellationToken);
    }
}