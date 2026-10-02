using FluentValidation;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.songs.commands;

public sealed class DeleteSongCategoryMappingCommand : IRequest<bool>
{
    public int SongId { get; set; }
    public int CategoryId { get; set; }
}

public sealed class DeleteSongCategoryMappingCommandValidator : AbstractValidator<DeleteSongCategoryMappingCommand>
{
    public DeleteSongCategoryMappingCommandValidator()
    {
        RuleFor(x => x.SongId).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public sealed class DeleteSongCategoryMappingHandler : IRequestHandler<DeleteSongCategoryMappingCommand, bool>
{
    private readonly ISongRepository _repository;
    private readonly ISongQueries _queries;

    public DeleteSongCategoryMappingHandler(ISongRepository repository, ISongQueries queries)
    {
        _repository = repository;
        _queries = queries;
    }

    public async Task<bool> Handle(DeleteSongCategoryMappingCommand request, CancellationToken cancellationToken)
    {
        if (!await _queries.SongCategoryMappingExistsAsync(request.SongId, request.CategoryId, cancellationToken))
            throw new NotFoundException("Liên kết bài hát và thể loại không tồn tại.");

        return await _repository.DeleteSongCategoryMappingAsync(request.SongId, request.CategoryId, cancellationToken);
    }
}
