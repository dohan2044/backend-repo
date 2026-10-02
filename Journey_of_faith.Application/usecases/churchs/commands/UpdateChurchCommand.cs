using MediatR;
using Microsoft.AspNetCore.Http;

namespace Journey_of_faith.Application.usecases.churchs.commands;


public class UpdateChurchCommand : IRequest<int>
{
    public int Id {get; set;}
    public string? Name {get; set;}
    public string? Email {get; set;}
    public  string? Address {get; set;}
    public int DioceseId {get; set;}
    public float? Longitude {get; set;}
    public float? Latitude {get; set;}
    public string? Boss {get; set;}
    public string? Description {get; set;}
    public Guid UserId {get; set;}
    public List<string> ChurchImages {get; set;} = [];
    public List<IFormFile>? Files { get; set; }
}
