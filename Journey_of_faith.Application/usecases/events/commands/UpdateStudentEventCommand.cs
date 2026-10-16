using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.events.commands;

public sealed class UpdateStudentEventCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
    public int? MaxParticipants { get; init; }
    public DateTime? RegistrationDeadline { get; init; }
    public string? CoverImageUrl { get; init; }
    public string[] CacheGroups => ["student-events"];
}

public sealed class UpdateStudentEventCommandValidator : AbstractValidator<UpdateStudentEventCommand>
{
    public UpdateStudentEventCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Title).NotEmpty().MaximumLength(300);
        RuleFor(command => command.Description).NotEmpty();
        RuleFor(command => command.Location).NotEmpty().MaximumLength(300);
        RuleFor(command => command.EndAt).GreaterThan(command => command.StartAt);
        RuleFor(command => command.MaxParticipants).GreaterThan(0).When(command => command.MaxParticipants.HasValue);
        RuleFor(command => command.RegistrationDeadline)
            .LessThanOrEqualTo(command => command.StartAt)
            .When(command => command.RegistrationDeadline.HasValue);
        RuleFor(command => command.CoverImageUrl).MaximumLength(500);
    }
}

public sealed class UpdateStudentEventHandler(
    IStudentEventRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateStudentEventCommand, Unit>
{
    public async Task<Unit> Handle(UpdateStudentEventCommand request, CancellationToken cancellationToken)
    {
        var studentEvent = repository.GetStudentEventById(request.Id)
            ?? throw new NotFoundException($"Student event {request.Id} was not found.");

        studentEvent.UpdateDetails(
            request.Title,
            request.Description,
            request.Location,
            request.StartAt,
            request.EndAt,
            request.MaxParticipants,
            request.RegistrationDeadline,
            request.CoverImageUrl);

        repository.UpdateStudentEvent(studentEvent);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
