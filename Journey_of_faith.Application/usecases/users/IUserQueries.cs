using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.users;

public interface IUserQueries
{
    Task<PagedResult<UserResponseDto>> GetUsersAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<UserResponseDto?> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
}
