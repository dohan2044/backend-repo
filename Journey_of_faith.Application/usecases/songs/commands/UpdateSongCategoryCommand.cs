using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public sealed class UpdateSongCategoryCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class UpdateSongCategoryCommandValidator : AbstractValidator<UpdateSongCategoryCommand>
{
    public UpdateSongCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateSongCategoryHandler : IRequestHandler<UpdateSongCategoryCommand, bool>
{
    private readonly ISongRepository _repository;
    private readonly ISongQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public UpdateSongCategoryHandler(ISongRepository repository, ISongQueries queries, ICurrentUserService currentUser)
    {
        _repository = repository;
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateSongCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizationException("Người dùng không hợp lệ.");
        if (!await _queries.SongCategoryExistsAsync(request.Id, cancellationToken))
            throw new NotFoundException("Thể loại bài hát không tồn tại.");

        var name = request.Name.Trim();
        var categories = await _queries.GetSongCategoriesAsync(cancellationToken);
        if (categories.Any(x => x.Id != request.Id && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new ConfictException("Tên thể loại bài hát đã tồn tại.");

        return await _repository.UpdateSongCategoryAsync(request.Id, new SongCategory(name), userId, cancellationToken);
    }
}
