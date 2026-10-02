using FluentValidation;
using Journey_of_faith.Domain.entities.catholic;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Application.common.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands;

#nullable disable
public class CreateDailyWordCommand : IRequest<bool>
{
    public DateTime Date {get; set;}
    public string Title {get; set;}
    public string Content {get; set;}
    public string Gospel {get; set;}
}

public class CreateDailyWordValidator : AbstractValidator<CreateDailyWordCommand>
{
    public CreateDailyWordValidator()
    {
        RuleFor(e => e.Date).NotEmpty().WithMessage("Date is not empty.")
        .Must(e => e.Date >= DateTime.Now).WithMessage("Khong the la ngay trong qua khu");

        RuleFor(e => e.Content).NotEmpty().WithMessage("Content is not empty.");
        RuleFor(e => e.Title).NotEmpty().WithMessage("Title is not empty.")
            .Length(200).WithMessage("is not greate 200 char");

    }
}


public class CreateDailyWordHandler : IRequestHandler< CreateDailyWordCommand,bool>
{
    private readonly IChurchRepository churchRepository;
    private readonly IUnitOfWork unitOfWork;
    public CreateDailyWordHandler(IChurchRepository churchRepository, IUnitOfWork unitOfWork)
    {
        this.churchRepository = churchRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateDailyWordCommand command, CancellationToken cancellationToken)
    {
        var dailyWord = new DailyWord(command.Date, command.Title, command.Content, command.Gospel);
        await churchRepository.AddAsync(dailyWord, cancellationToken);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return true;
    }
}
