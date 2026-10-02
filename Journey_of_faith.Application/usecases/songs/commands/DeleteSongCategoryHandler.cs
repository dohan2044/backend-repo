using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class DeleteSongCategoryHandler : IRequestHandler<DeleteSongCategoryCommand, bool>
{
    private readonly ISongRepository _songRepository;
    private readonly ICurrentUserService currentUserService;
    public DeleteSongCategoryHandler(ISongRepository songRepository, ICurrentUserService currentUserService)
    {
        _songRepository = songRepository;
        this.currentUserService = currentUserService;
    }


    public async Task<bool> Handle(DeleteSongCategoryCommand command, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Người dùng không hợp lệ");
        }
        return await _songRepository.DeleteSongCategoryAsync(command.Id, user, cancellationToken);
    }
}