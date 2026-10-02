using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Application.behaviors;

public class LoggingRequestBehavior<TRequest, TResponse>(
    ILogger<LoggingRequestBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken token)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N");
        var stopwatch = Stopwatch.StartNew();

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestName"] = requestName
        });

        logger.LogInformation("Handling application request {RequestName}", requestName);

        try
        {
            var response = await next();
            stopwatch.Stop();
            logger.LogInformation(
                "Handled application request {RequestName} in {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning(
                "Application request {RequestName} was cancelled after {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogError(
                exception,
                "Application request {RequestName} failed after {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
