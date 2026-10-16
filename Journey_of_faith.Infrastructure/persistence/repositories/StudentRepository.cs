using Domain.Entities;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace Journey_of_faith.Infrastructure.persistence.repositories;

public sealed class StudentRepository(ApplicationDbContext context) : IStudentGroupRepository
{
    public async Task AddGroup(StudentGroup group, CancellationToken token = default)
        => await context.StudentGroups.AddAsync(group, token);

    public async Task AddGroupMember(StudentGroupMember groupMember, CancellationToken token = default)
    {
        var group = await context.StudentGroups
            .Include(item => item.Members)
            .FirstOrDefaultAsync(item => item.Id == groupMember.GroupId, token);

        if (group is null)
        {
            throw new InvalidOperationException($"Student group {groupMember.GroupId} was not found.");
        }

        group.AddMember(groupMember);
    }

    public void UpdateGroup(StudentGroup group) => context.StudentGroups.Update(group);

    public void AssignGroupMemberRole(StudentGroupMember groupMember)
        => context.StudentGroupMembers.Update(groupMember);

    public void DeleteGroup(StudentGroup group)
    {
        group.IsDeleted = true;
        group.DeletionTime = DateTime.UtcNow;
        context.StudentGroups.Update(group);
    }

    public void DeleteGroupMember(StudentGroupMember groupMember)
    {
        groupMember.IsDeleted = true;
        groupMember.DeletionTime = DateTime.UtcNow;
        context.StudentGroupMembers.Update(groupMember);
    }

    public StudentGroup? GetGroupById(int groupId)
        => context.StudentGroups
            .Include(group => group.Members.Where(member => member.IsDeleted != true))
            .FirstOrDefault(group => group.Id == groupId && group.IsDeleted != true);

    public IEnumerable<StudentGroup> Groups(int page = 1, int pageSize = 15, string? textSearch = null)
    {
        ValidatePage(page, pageSize);

        var query = context.StudentGroups
            .AsNoTracking()
            .Where(group => group.IsDeleted != true);

        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            var search = textSearch.Trim();
            query = query.Where(group => group.Name.Contains(search) || group.Code.Contains(search));
        }

        return query
            .OrderBy(group => group.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public IEnumerable<StudentGroupMember> GroupMembers(
        int groupId,
        int page = 1,
        int pageSize = 15,
        string? textSearch = null)
    {
        ValidatePage(page, pageSize);

        var query = context.StudentGroupMembers
            .AsNoTracking()
            .Where(member => member.GroupId == groupId && member.IsDeleted != true);

        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            var search = textSearch.Trim();
            query = query.Where(member => member.Role.Contains(search));
        }

        return query
            .OrderBy(member => member.JoinedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    }
}
