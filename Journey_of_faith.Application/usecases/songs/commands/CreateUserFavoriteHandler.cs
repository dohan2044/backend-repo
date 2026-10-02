using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateUserFavoriteHandler : IRequestHandler<CreateUserFavoriteCommand, int>
{
    private readonly ISongRepository songRepository;
    private readonly ICurrentUserService currentUserService;
    public CreateUserFavoriteHandler(ISongRepository repository, ICurrentUserService currentUserService)
    {
        this.songRepository = repository;
        this.currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateUserFavoriteCommand command, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Người dùng không hợp lệ");
        }

        var userFavorite = new UserFavoriteSong(user, command.SongId);
        return await songRepository.CreateUserFavoriteSongAsync(userFavorite, cancellationToken);
    }
}