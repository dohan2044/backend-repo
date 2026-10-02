using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;


public class CreateListeningHistoryCommand : IRequest<int>
{
    public int UserId {get; set;}
    public int SongId {get; set;}
}