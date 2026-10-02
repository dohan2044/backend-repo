using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateArtistHandler : IRequestHandler<CreateArtistCommand, int> 
{
    private readonly ISongRepository _songRepository;
    public CreateArtistHandler(ISongRepository songRepository)
    {
        _songRepository = songRepository;
    }

    public async Task<int> Handle(CreateArtistCommand command, CancellationToken cancellationToken)
    {
        var artist = new Artist(command.Name, command.Description, command.ImageUrl);
        return await _songRepository.CreateArtistAsync(artist: artist, cancellationToken: cancellationToken);
    }
}