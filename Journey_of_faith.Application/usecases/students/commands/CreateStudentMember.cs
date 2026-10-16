using Domain.Entities;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.users;
using Journey_of_faith.Domain.interfaces;
using MediatR;

namespace Journey_of_faith.Application.usecases.students.commands;

public class CreateStudentMemberCommand : IRequest<Unit>, ICacheInvalidCommand
{
    public int GroupId {get; set;}
    public Guid UserId {get; set;}
    public string[] CacheGroups => ["students-group"];
}



public class CreateStudentMemberHandler : IRequestHandler<CreateStudentMemberCommand, Unit>
{
    private readonly IStudentGroupRepository _studentGroupRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentQueries _studentQueries;
    private readonly IUserQueries _userQueries;
    public CreateStudentMemberHandler(IStudentGroupRepository student, IUnitOfWork unitOfWork, IStudentQueries studentQueries, IUserQueries userQueries)
    {
        _studentGroupRepo = student;
        _unitOfWork = unitOfWork;
        _studentQueries = studentQueries;
        _userQueries = userQueries;
    }

    public async Task<Unit> Handle(CreateStudentMemberCommand request, CancellationToken cancellationToken)
    {
        if(!await _studentQueries.ExistsGroupIdAsync(request.GroupId, cancellationToken)) 
            throw new NotFoundException($"Student group with id {request.GroupId} not found");

        if(await _userQueries.GetUserAsync(request.UserId, cancellationToken) is null)
            throw new NotFoundException($"User with id {request.UserId} not found");

        var groupMember = new StudentGroupMember(request.GroupId, request.UserId);
        await _studentGroupRepo.AddGroupMember(groupMember, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return Unit.Value;
    }
}