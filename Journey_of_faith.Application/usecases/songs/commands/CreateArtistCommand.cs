using FluentValidation;
using Journey_of_faith.Application.usecases.songs;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public class CreateArtistCommand: IRequest<int>
{
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string ImageUrl {get; set;} = string.Empty;
}


public class CreateArtistValidator: AbstractValidator<CreateArtistCommand>
{
    private readonly ISongQueries _songQueries;
    public CreateArtistValidator(ISongQueries songQueries)
    {
        _songQueries = songQueries;

        RuleFor(e => e.Name)
            .NotNull().WithMessage("Name cann't null.")
            .NotEmpty().WithMessage("Name is not empty.")
            .MustAsync(async (name, cancellationToken) =>
            {
                var artist = await _songQueries.ArtistNameExistsAsync(name, cancellationToken);
                return !artist;
            }).WithMessage("Tên nghệ sĩ đã tồn tại, không thể thêm").WithErrorCode("Conflic");

        RuleFor(e => e.Description)
            .NotNull().WithMessage("Không được bỏ trống")
            .NotEmpty().WithMessage("Không duojcd bỏ trống")
            .MaximumLength(1000).WithMessage("Không được vượt quá 1000 ký tụe");
    }
}
