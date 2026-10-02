using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;
public class UpdateArtistCommand: IRequest<int>
{
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string ImageUrl {get; set;} = string.Empty;
}