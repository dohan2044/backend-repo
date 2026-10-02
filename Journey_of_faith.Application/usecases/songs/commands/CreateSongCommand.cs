using FluentValidation;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateSongCommand : IRequest<int>
{
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public int? AlbumId { get; set; }
    public int Duration { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? Lyric { get; set; }
    public int PlayCount { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategorySongId { get; set; }
}

public class CreateSongValidator : AbstractValidator<CreateSongCommand>
{
    public CreateSongValidator(ISongRepository _)
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ArtistId).GreaterThan(0);
        RuleFor(x => x.AlbumId).GreaterThan(0).When(x => x.AlbumId.HasValue);
        RuleFor(x => x.Duration).GreaterThan(0);
        RuleFor(x => x.AudioUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CoverImageUrl).MaximumLength(500);
        RuleFor(x => x.PlayCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategorySongId).GreaterThan(0);
    }
}
