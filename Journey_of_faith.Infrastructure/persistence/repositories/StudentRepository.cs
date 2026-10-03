using Domain.Entities;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.context;

namespace Journey_of_faith.Infrastructure.persistence.repositories;



public class StudentRepository(ApplicationDbContext _context) : IStudentGroupRepository
{
    public async Task AddGroup(StudentGroup group) => await _context.StudentGroups.AddAsync(group);
    public async Task AddGroupMember(StudentGroupMember groupMember)
    {
        await _context.StudentGroupMembers.AddAsync(groupMember);
    }
    public void UpdateGroup(StudentGroup group)
    {
        _context.StudentGroups.Update(group);
    }
    public async Task AssignGroupMemberRole(StudentGroupMember groupMember, string role)
    {
        _context.StudentGroupMembers.Update(groupMember);
    }

    public void DeleteGroup(StudentGroup group)
    {
        throw new NotImplementedException();
    }

    public void DeleteGroupMember(StudentGroupMember groupMember)
    {
        throw new NotImplementedException();
    }

    public StudentGroup GetGroupById(int groupId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<StudentGroup> Groups(int page = 1, int pageSize = 15, string textSearch = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<StudentGroupMember> GroupMembers(int groupId, int page = 1, int pageSize = 15, string textSearch = null)
    {
        throw new NotImplementedException();
    }
}
