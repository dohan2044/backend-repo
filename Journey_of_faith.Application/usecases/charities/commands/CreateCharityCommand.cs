using Domain.Entities;
using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.charities.commands;

public sealed class CreateCharityCommand : IRequest<int>, ICacheInvalidCommand
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
    public int? MaxParticipants { get; init; }
    public string? CoverImageUrl { get; init; }
    public string[] CacheGroups => ["charities"];
}

public sealed class CreateCharityCommandValidator : AbstractValidator<CreateCharityCommand>
{
    public CreateCharityCommandValidator()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(300);
        RuleFor(command => command.Description).NotEmpty();
        RuleFor(command => command.Location).NotEmpty().MaximumLength(300);
        RuleFor(command => command.EndAt).GreaterThan(command => command.StartAt);
        RuleFor(command => command.MaxParticipants).GreaterThan(0).When(command => command.MaxParticipants.HasValue);
        RuleFor(command => command.CoverImageUrl).MaximumLength(500);
    }
}

public sealed class CreateCharityHandler(
    ICharityRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCharityCommand, int>
{
    public async Task<int> Handle(CreateCharityCommand request, CancellationToken cancellationToken)
    {
        var charity = new CharityActivity(
            request.Title,
            request.Description,
            request.Location,
            request.StartAt,
            request.EndAt,
            request.MaxParticipants,
            request.CoverImageUrl);

        repository.AddCharity(charity);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return charity.Id;
    }
}
