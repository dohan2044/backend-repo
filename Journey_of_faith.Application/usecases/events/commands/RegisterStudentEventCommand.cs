using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.users;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using Domain.Entities;

namespace Journey_of_faith.Application.usecases.events.commands;

public sealed class RegisterStudentEventCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int EventId { get; init; }
    public Guid UserId { get; init; }
    public string[] CacheGroups => ["student-events"];
}

public sealed class RegisterStudentEventCommandValidator : AbstractValidator<RegisterStudentEventCommand>
{
    public RegisterStudentEventCommandValidator()
    {
        RuleFor(command => command.EventId).GreaterThan(0);
        RuleFor(command => command.UserId).NotEmpty();
    }
}

public sealed class RegisterStudentEventHandler(
    IStudentEventRepository repository,
    IStudentEventQueries queries,
    IUserQueries userQueries,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterStudentEventCommand, Unit>
{
    public async Task<Unit> Handle(RegisterStudentEventCommand request, CancellationToken cancellationToken)
    {
        if (!await queries.StudentEventExistsAsync(request.EventId, cancellationToken))
            throw new NotFoundException($"Student event {request.EventId} was not found.");
        if (await userQueries.GetUserAsync(request.UserId, cancellationToken) is null)
            throw new NotFoundException($"User {request.UserId} was not found.");
        if (await queries.RegistrationExistsAsync(request.EventId, request.UserId, cancellationToken))
            throw new ConfictException("User is already registered for this student event.");

        repository.RegisterStudentEvent(new StudentEventRegistration(request.EventId, request.UserId));
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
