using Domain.Entities;

namespace Journey_of_faith.Domain.interfaces;


public interface IStudentGroupRepository
{
    Task AddGroup(StudentGroup group);
    Task AddGroupMember(StudentGroupMember groupMember);
    void UpdateGroup(StudentGroup group);
    Task AssignGroupMemberRole(StudentGroupMember groupMember, string role);
    void DeleteGroup(StudentGroup group);
    void DeleteGroupMember(StudentGroupMember groupMember);
    StudentGroup? GetGroupById(int groupId);
    IEnumerable<StudentGroup> Groups(int page= 1, int pageSize = 15, string? textSearch = null);
    IEnumerable<StudentGroupMember> GroupMembers(int groupId, int page = 1, int pageSize = 15, string? textSearch = null);
}