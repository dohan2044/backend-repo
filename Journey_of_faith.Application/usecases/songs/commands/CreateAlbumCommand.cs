using System.Data;
using FluentValidation;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateAlbumCommand : IRequest<int>
{
    public string Title {get; set;} = string.Empty;
    public int ArtistId {get; set;}
    public int ReleaseYear {get; set;}
    public string CoverImageUrl {get; set;} = string.Empty;
}

public class CreateAlbumValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumValidator()
    {
        RuleFor(e => e.Title)
            .NotNull().WithMessage("Album title is not null.")
            .NotEmpty().WithMessage("Album title is not empty.");

        RuleFor(e => e.ArtistId)
            .NotNull().WithMessage("Artist is not null")
            .NotEmpty().WithMessage("ArtistId is not empty.")
            .LessThan(0).WithMessage("ArtistId is not equals zero.");

        RuleFor(e => e.ReleaseYear)
            .NotNull().WithMessage("ReleaseYear is not null.")
            .NotEmpty().WithMessage("ReleaseYear is not empty.")
            .LessThan(2000).WithMessage("ReleaseYear is not less than 2000")
            .GreaterThan(2026).WithMessage("ReleaseYear is not greater than 2026");
    }
}