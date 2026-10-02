using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands;


public class DeleteChurchCommand : IRequest<bool>
{
    public int Id {get; set;}
    public bool? Force {get; set;}
}


public class DeleteChurchHandler : IRequestHandler<DeleteChurchCommand, bool>
{
    private readonly IChurchRepository churchRepository;
    private readonly IUnitOfWork unitOfWork;
    public DeleteChurchHandler(IChurchRepository churchRepository, IUnitOfWork unitOfWork)
    {
        this.churchRepository = churchRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteChurchCommand command, CancellationToken cancellationToken)
    {
        bool isDeleted = await churchRepository.DeleteChurchAsync(
            command.Id,
            command.Force ?? false,
            cancellationToken);
        if(!isDeleted)
        {
            throw new NotFoundException($"Nhà thờ với mã: {command.Id} không hợp lệ");
        }

        await unitOfWork.SaveChangeAsync(cancellationToken);
        return true;
    }
}
