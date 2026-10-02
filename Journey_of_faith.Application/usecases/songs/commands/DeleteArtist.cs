using System.Runtime.CompilerServices;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class DeleteArtistCommand : IRequest<bool>
{
    public int Id {get; set;}
}

public class DeleteArtistHandler : IRequestHandler<DeleteArtistCommand, bool>
{
    private readonly ISongRepository songRepository;
    private readonly ICurrentUserService currentUserService;
    public DeleteArtistHandler(ISongRepository songRepository, ICurrentUserService currentUserService)
    {
        this.songRepository = songRepository;
        this.currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteArtistCommand command, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Người dùng khum hợp lệ");   
        }
        return await songRepository.DeleteArtistAsync(command.Id, user, cancellationToken: cancellationToken);
    }
}