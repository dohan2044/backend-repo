using FluentValidation;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public sealed class CreateSongCategoryMappingCommand : IRequest<int>
{
    public int SongId { get; set; }
    public int CategoryId { get; set; }
}

public sealed class CreateSongCategoryMappingCommandValidator : AbstractValidator<CreateSongCategoryMappingCommand>
{
    public CreateSongCategoryMappingCommandValidator()
    {
        RuleFor(x => x.SongId).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public sealed class CreateSongCategoryMappingHandler : IRequestHandler<CreateSongCategoryMappingCommand, int>
{
    private readonly ISongRepository _repository;
    private readonly ISongQueries _queries;

    public CreateSongCategoryMappingHandler(ISongRepository repository, ISongQueries queries)
    {
        _repository = repository;
        _queries = queries;
    }

    public async Task<int> Handle(CreateSongCategoryMappingCommand request, CancellationToken cancellationToken)
    {
        if (!await _queries.SongExistsAsync(request.SongId, cancellationToken))
            throw new NotFoundException("Bài hát không tồn tại.");
        if (!await _queries.SongCategoryExistsAsync(request.CategoryId, cancellationToken))
            throw new NotFoundException("Thể loại bài hát không tồn tại.");
        if (await _queries.SongCategoryMappingExistsAsync(request.SongId, request.CategoryId, cancellationToken))
            throw new ConfictException("Bài hát đã thuộc thể loại này.");

        return await _repository.CreateSongCategoryMappingAsync(
            new SongCategoryMapping(request.SongId, request.CategoryId), cancellationToken);
    }
}
