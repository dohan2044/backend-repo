using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.charities.commands;

public sealed class UpdateCharityCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
    public int? MaxParticipants { get; init; }
    public string? CoverImageUrl { get; init; }
    public string[] CacheGroups => ["charities"];
}

public sealed class UpdateCharityCommandValidator : AbstractValidator<UpdateCharityCommand>
{
    public UpdateCharityCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Title).NotEmpty().MaximumLength(300);
        RuleFor(command => command.Description).NotEmpty();
        RuleFor(command => command.Location).NotEmpty().MaximumLength(300);
        RuleFor(command => command.EndAt).GreaterThan(command => command.StartAt);
        RuleFor(command => command.MaxParticipants).GreaterThan(0).When(command => command.MaxParticipants.HasValue);
        RuleFor(command => command.CoverImageUrl).MaximumLength(500);
    }
}

public sealed class UpdateCharityHandler(
    ICharityRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCharityCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCharityCommand request, CancellationToken cancellationToken)
    {
        var charity = repository.GetCharityById(request.Id)
            ?? throw new NotFoundException($"Charity activity {request.Id} was not found.");

        charity.UpdateDetails(
            request.Title,
            request.Description,
            request.Location,
            request.StartAt,
            request.EndAt,
            request.MaxParticipants,
            request.CoverImageUrl);

        repository.UpdateCharity(charity);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}
