#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Infrastructure.common;

public class InfrastructureLoggingProxy<TService> : DispatchProxy
    where TService : class
{
    private static readonly MethodInfo WrapGenericTaskMethod = typeof(InfrastructureLoggingProxy<TService>)
        .GetMethod(nameof(WrapGenericTaskAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private TService _decorated = null!;
    private ILogger<InfrastructureLoggingProxy<TService>> _logger = null!;

    public static TService Create(
        TService decorated,
        ILogger<InfrastructureLoggingProxy<TService>> logger)
    {
        var proxy = DispatchProxy.Create<TService, InfrastructureLoggingProxy<TService>>();
        var loggingProxy = (InfrastructureLoggingProxy<TService>)(object)proxy;
        loggingProxy._decorated = decorated;
        loggingProxy._logger = logger;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);

        var serviceName = typeof(TService).Name;
        var methodName = targetMethod.Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(
            "Executing infrastructure operation {InfrastructureService}.{InfrastructureMethod}",
            serviceName,
            methodName);

        try
        {
            var result = targetMethod.Invoke(_decorated, args);

            if (result is Task task)
            {
                if (targetMethod.ReturnType.IsGenericType)
                {
                    var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
                    return WrapGenericTaskMethod
                        .MakeGenericMethod(resultType)
                        .Invoke(this, [task, serviceName, methodName, stopwatch])!;
                }

                return WrapTaskAsync(task, serviceName, methodName, stopwatch);
            }

            LogCompletion(serviceName, methodName, stopwatch);
            return result;
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            LogFailure(exception.InnerException, serviceName, methodName, stopwatch);
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
        catch (Exception exception)
        {
            LogFailure(exception, serviceName, methodName, stopwatch);
            throw;
        }
    }

    private async Task WrapTaskAsync(
        Task task,
        string serviceName,
        string methodName,
        Stopwatch stopwatch)
    {
        try
        {
            await task.ConfigureAwait(false);
            LogCompletion(serviceName, methodName, stopwatch);
        }
        catch (Exception exception)
        {
            LogFailure(exception, serviceName, methodName, stopwatch);
            throw;
        }
    }

    private async Task<TResult> WrapGenericTaskAsync<TResult>(
        Task task,
        string serviceName,
        string methodName,
        Stopwatch stopwatch)
    {
        try
        {
            var result = await ((Task<TResult>)task).ConfigureAwait(false);
            LogCompletion(serviceName, methodName, stopwatch);
            return result;
        }
        catch (Exception exception)
        {
            LogFailure(exception, serviceName, methodName, stopwatch);
            throw;
        }
    }

    private void LogCompletion(string serviceName, string methodName, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds >= 1_000)
        {
            _logger.LogWarning(
                "Slow infrastructure operation {InfrastructureService}.{InfrastructureMethod} completed in {ElapsedMilliseconds} ms",
                serviceName,
                methodName,
                stopwatch.ElapsedMilliseconds);
            return;
        }

        _logger.LogInformation(
            "Infrastructure operation {InfrastructureService}.{InfrastructureMethod} completed in {ElapsedMilliseconds} ms",
            serviceName,
            methodName,
            stopwatch.ElapsedMilliseconds);
    }

    private void LogFailure(
        Exception exception,
        string serviceName,
        string methodName,
        Stopwatch stopwatch)
    {
        stopwatch.Stop();
        _logger.LogError(
            exception,
            "Infrastructure operation {InfrastructureService}.{InfrastructureMethod} failed after {ElapsedMilliseconds} ms",
            serviceName,
            methodName,
            stopwatch.ElapsedMilliseconds);
    }
}

public static class InfrastructureLoggingRegistration
{
    public static IServiceCollection AddLoggedScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped<TService>(serviceProvider =>
            InfrastructureLoggingProxy<TService>.Create(
                serviceProvider.GetRequiredService<TImplementation>(),
                serviceProvider.GetRequiredService<ILogger<InfrastructureLoggingProxy<TService>>>()));

        return services;
    }
}
