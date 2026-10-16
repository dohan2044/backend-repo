using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.charities.commands;

public sealed class DeleteCharityCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int Id { get; init; }
    public string[] CacheGroups => ["charities"];
}

public sealed class DeleteCharityCommandValidator : AbstractValidator<DeleteCharityCommand>
{
    public DeleteCharityCommandValidator()
        => RuleFor(command => command.Id).GreaterThan(0);
}

public sealed class DeleteCharityHandler(
    ICharityRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCharityCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCharityCommand request, CancellationToken cancellationToken)
    {
        var charity = repository.GetCharityById(request.Id)
            ?? throw new NotFoundException($"Charity activity {request.Id} was not found.");

        repository.DeleteCharity(charity);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
