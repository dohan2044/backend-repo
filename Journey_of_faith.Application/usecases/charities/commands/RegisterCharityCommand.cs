using Domain.Entities;
using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.users;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.charities.commands;

public sealed class RegisterCharityCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int CharityId { get; init; }
    public Guid UserId { get; init; }
    public string? Note { get; init; }
    public string[] CacheGroups => ["charities"];
}

public sealed class RegisterCharityCommandValidator : AbstractValidator<RegisterCharityCommand>
{
    public RegisterCharityCommandValidator()
    {
        RuleFor(command => command.CharityId).GreaterThan(0);
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.Note).MaximumLength(500);
    }
}

public sealed class RegisterCharityHandler(
    ICharityRepository repository,
    ICharityQueries queries,
    IUserQueries userQueries,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCharityCommand, Unit>
{
    public async Task<Unit> Handle(RegisterCharityCommand request, CancellationToken cancellationToken)
    {
        if (!await queries.CharityExistsAsync(request.CharityId, cancellationToken))
            throw new NotFoundException($"Charity activity {request.CharityId} was not found.");
        if (await userQueries.GetUserAsync(request.UserId, cancellationToken) is null)
            throw new NotFoundException($"User {request.UserId} was not found.");
        if (await queries.RegistrationExistsAsync(request.CharityId, request.UserId, cancellationToken))
            throw new ConfictException("User is already registered for this charity activity.");

        repository.RegistrationChatiry(
            new CharityRegistration(request.CharityId, request.UserId, request.Note));
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
