using Domain.Entities;

namespace Journey_of_faith.Domain.interfaces;


public interface IStudentGroupRepository
{
    Task AddGroup(StudentGroup group, CancellationToken token = default);
    Task AddGroupMember(StudentGroupMember groupMember, CancellationToken token = default);
    void UpdateGroup(StudentGroup group);
    void AssignGroupMemberRole(StudentGroupMember groupMember);
    void DeleteGroup(StudentGroup group);
    void DeleteGroupMember(StudentGroupMember groupMember);
    StudentGroup? GetGroupById(int groupId);
    IEnumerable<StudentGroup> Groups(int page= 1, int pageSize = 15, string? textSearch = null);
    IEnumerable<StudentGroupMember> GroupMembers(int groupId, int page = 1, int pageSize = 15, string? textSearch = null);
}