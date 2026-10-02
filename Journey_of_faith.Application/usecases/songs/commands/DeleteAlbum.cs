using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class DeleteAlbumCommand : IRequest<bool>
{
    public int Id {get; set;}
}


public class DeleteAlbumHandler : IRequestHandler<DeleteAlbumCommand, bool>
{
    private readonly ISongRepository songRepository;
    private readonly ICurrentUserService currentUserService;
    public DeleteAlbumHandler(ISongRepository songRepository, ICurrentUserService currentUserService)
    {
        this.songRepository = songRepository;   
        this.currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteAlbumCommand command, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Người dùng khong hợp lệ");
        }
        return await songRepository.DeleteAlbumAsync(command.Id, userId: user, cancellationToken);
    }
}