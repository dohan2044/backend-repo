using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;


public class CreateListeningHistoryHandler : IRequestHandler<CreateListeningHistoryCommand, int>
{
    private readonly ISongRepository songRepository;
    private readonly ICurrentUserService currentUserService;
    public CreateListeningHistoryHandler(ISongRepository songRepository, ICurrentUserService currentUserService)
    {
        this.songRepository = songRepository;
        this.currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateListeningHistoryCommand command, CancellationToken cancellationToken)
    {
        if(Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Vui lòng đăng nhập lại");
        }

        var listenHistory = new ListeningHistory(user, command.SongId);
        return await songRepository.CreateListeningHistoryAsync(listenHistory, cancellationToken);
    }
}