using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Application.common.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands;

public class CreateLiturgy : CreateLiturgyCommand, IRequest<bool>
{
    public DateTime DateActive {get; set;}
}


public class CreateLiturgyHandler : IRequestHandler<CreateLiturgy, bool>
{
    private readonly IChurchRepository churchRepository;
    private readonly IUnitOfWork unitOfWork;
    public CreateLiturgyHandler(IChurchRepository churchRepository, IUnitOfWork unitOfWork)
    {
        this.churchRepository = churchRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateLiturgy command, CancellationToken cancellationToken)
    {
        var liturgy = new Liturgy(
            command.Reading,
            command.ResponsorialPsalm,
            command.Gospel,
            command.EndWord,
            command.DateActive);
        await churchRepository.AddAsync(liturgy, cancellationToken);
        await unitOfWork.SaveChangeAsync(cancellationToken);
        return true;
    }
}
