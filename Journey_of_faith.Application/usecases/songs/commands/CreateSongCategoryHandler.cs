using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;


public class CreateSongCategoryHandler : IRequestHandler<CreateSongCategoryCommand, int>
{
    private readonly ISongRepository _repo;
    public CreateSongCategoryHandler(ISongRepository repo)
    {
        _repo = repo;
    }


    public async Task<int> Handle(CreateSongCategoryCommand command, CancellationToken cancellationToken)
    {
        var insert = new SongCategory(command.Name.Trim());
        return await _repo.CreateSongCategoryAsync(insert, cancellationToken);
    }
}
