using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class DeleteSongCategoryCommand : IRequest<bool>
{
    public int Id {get; set;}
}
