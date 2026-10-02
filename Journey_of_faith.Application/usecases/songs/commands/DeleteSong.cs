using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class DeleteSongCommand : IRequest<bool>
{
    public int Id {get; set;}
}


public class DeleteSongValidator : AbstractValidator<DeleteSongCommand>
{
    public DeleteSongValidator()
    {
        RuleFor(e => e.Id)
            .GreaterThan(0).WithMessage("Id Phải lớn hơn 0");
    }
}



public class DeleteSongHandler : IRequestHandler<DeleteSongCommand, bool>
{
    private readonly ISongRepository songRepository;
    private readonly ICurrentUserService currentUserService;
    public DeleteSongHandler(ISongRepository songRepository, ICurrentUserService currentUserService)
    {
        this.songRepository = songRepository;
        this.currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteSongCommand command, CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(currentUserService.UserId, out var user))
        {
            throw new UnauthorizationException("Người dùng không hợp lệ");
        }

        if(currentUserService.GetRoleUserName is not "Admin")
        {
            throw new ForbiddenException("Bạn không có quền xóa bài hát này");
        }
        return await songRepository.DeleteSongAsync(command.Id, user, cancellationToken);
    }
}