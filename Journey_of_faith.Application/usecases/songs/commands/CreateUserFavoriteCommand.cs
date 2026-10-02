using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateUserFavoriteCommand : IRequest<int>
{
    public int SongId {get; set;}
    public DateTime CreateTime {get; set;}
}