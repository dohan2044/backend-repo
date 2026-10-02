using System.Data.Common;
using Journey_of_faith.Application.common.dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Application.usecases.users.queries;

public class GetUserDetailsHandler : IRequestHandler<GetUserDetailsQuery, UserResponseDto?>
{
    private readonly ILogger<GetUserDetailsHandler> _logger;
    private readonly IUserQueries _userQueries;
    public GetUserDetailsHandler(ILogger<GetUserDetailsHandler> logger, IUserQueries userQueries)
    {
        _userQueries = userQueries;
        _logger = logger;
    }

    public async Task<UserResponseDto?> Handle(GetUserDetailsQuery query, CancellationToken token)
    {
        try
        {
            return await _userQueries.GetUserAsync(query.Id, token);
        } catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Request timeout to server.");
            throw;
        } catch (DbException ex)
        {
            _logger.LogError(ex, "Error connect db"); throw;
        }
    }
}
