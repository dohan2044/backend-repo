using Journey_of_faith.Domain.interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Journey_of_faith.Application.exceptions;
using Domain.Entities;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.untils;
namespace Journey_of_faith.Application.usecases.students.commands;


public class CreateGroupStudent : IRequest<Unit>
{
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string Url {get; set;} = string.Empty;
    public string EmailContract {get; set;} = string.Empty;
}

public class CreateGroupHandler : IRequestHandler<CreateGroupStudent, Unit>
{
    private readonly IStudentGroupRepository _studentGroupRepository;
    private readonly IStudentQueries _studentQueries;
    private readonly IUnitOfWork _unitOfWork;
    public CreateGroupHandler(IStudentGroupRepository studentGroupRepository, IStudentQueries studentQueries, IUnitOfWork unitOfWork)
    {
        _studentGroupRepository = studentGroupRepository;
        _studentQueries = studentQueries;
        _unitOfWork = unitOfWork;
    }


    public async Task<Unit> Handle(CreateGroupStudent request, CancellationToken cancellationToken)
    {
        if(await _studentQueries.ExistsGroupNameAsync(request.Name))
        {
            throw new Journey_of_faith.Application.exceptions.ConfictException($"Group name {request.Name} already exists");
        }
        var group = new StudentGroup(request.Name, request.Name.Substring(5)
        .RemoveVietnameseSigns().ToUpperInvariant().Replace(" ", "_"), request.Description, request.Url, request.EmailContract);
        await _studentGroupRepository.AddGroup(group);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return Unit.Value;
    }
}