using FluentValidation;
using Journey_of_faith.Application.usecases.songs;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;


public class CreateSongCategoryCommand : IRequest<int>
{
    public required string Name {get; set;}
}

public class CreateSongCategoryCommandValidator : AbstractValidator<CreateSongCategoryCommand>
{
    private readonly ISongQueries songQueries;
    public CreateSongCategoryCommandValidator(ISongQueries songQueries)
    {
        this.songQueries = songQueries;

        RuleFor(e => e.Name)
        .NotEmpty().WithMessage("Tên thể loại không được để trống.")
        .MaximumLength(200).WithMessage("Tên thể loại không được vượt quá 200 ký tự.")
        .MustAsync(async (name, cancellationToken) =>
        {
            bool exists = await songQueries.SongCategoryExistsAsync(name.Trim(), cancellationToken);
            return !exists;
        }).WithMessage("SongCategory already taken.");
    }
}
