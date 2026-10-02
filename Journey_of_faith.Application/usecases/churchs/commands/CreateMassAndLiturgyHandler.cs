using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.masslive;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands;

public class CreateMassAndLiturgyHandler : IRequestHandler<CreateMassAndLiturgyCommand, Unit>
{
    private readonly IChurchRepository _churchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMassAndLiturgyHandler(
        IChurchRepository churchRepository,
        IUnitOfWork unitOfWork)
    {
        _churchRepository = churchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CreateMassAndLiturgyCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var massSchedulesToCreate = new List<MassSchedule>();
        foreach(var item in command.Items)
        {
            var date = item.MassScheduleCreate.Date.HasValue ? item.MassScheduleCreate.Date.Value : (DateOnly?)null;
            var time = string.IsNullOrEmpty(item.MassScheduleCreate.Time) ? default : TimeOnly.Parse(item.MassScheduleCreate.Time);
            
            var mass = new MassSchedule(item.MassScheduleCreate.ChurchId, date, time.ToString(), 1, item.MassScheduleCreate.Name);

            var liturgy = new Liturgy(item.LiturgyCreate.Reading, item.LiturgyCreate.ResponsorialPsalm, item.LiturgyCreate.Gospel, item.LiturgyCreate.EndWord);

            mass.SetLiturgy(liturgy);

            massSchedulesToCreate.Add(mass);
        }
        
        await _churchRepository.AddMassSchedulesAsync(massSchedulesToCreate, cancellationToken);
        await _unitOfWork.SaveChangeAsync();
        return Unit.Value;
    }
}
