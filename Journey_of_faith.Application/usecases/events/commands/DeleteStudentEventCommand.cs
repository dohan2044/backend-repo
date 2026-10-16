using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands;

public sealed class DeleteStudentEventCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int Id { get; init; }
    public string[] CacheGroups => ["student-events"];
}

public sealed class DeleteStudentEventCommandValidator : AbstractValidator<DeleteStudentEventCommand>
{
    public DeleteStudentEventCommandValidator()
        => RuleFor(command => command.Id).GreaterThan(0);
}

public sealed class DeleteStudentEventHandler(
    IStudentEventRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteStudentEventCommand, Unit>
{
    public async Task<Unit> Handle(DeleteStudentEventCommand request, CancellationToken cancellationToken)
    {
        var studentEvent = repository.GetStudentEventById(request.Id)
            ?? throw new NotFoundException($"Student event {request.Id} was not found.");

        repository.DeleteStudentEvent(studentEvent);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
