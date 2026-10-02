using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Asp.Versioning;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Api.Controllers;
using Journey_of_faith.Api.middlewares;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Infrastructure.scheduling;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;

namespace IntegrationTesting.Controllers;

public sealed class NotificationSchedulesControllerTests : IAsyncLifetime
{
    private const string Route = "/api/v1/notifications/schedules";
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private IScheduler _quartz = null!;

    public async Task InitializeAsync()
    {
        var schedulerFactory = new StdSchedulerFactory(new NameValueCollection
        {
            ["quartz.scheduler.instanceName"] = $"notification-http-tests-{Guid.NewGuid():N}",
            ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz",
            ["quartz.threadPool.maxConcurrency"] = "1"
        });
        // Never start Quartz: HTTP tests must not execute notification jobs.
        _quartz = await schedulerFactory.GetScheduler();

        // No Program startup, appsettings, database, credentials or Firebase registration.
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers().AddApplicationPart(typeof(NotificationSchedulesController).Assembly);
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(), new HeaderApiVersionReader());
        }).AddMvc();
        builder.Services.AddAuthentication(TestAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName, _ => { });
        builder.Services.AddNotificationSchedulingAuthorization();
        builder.Services.AddSingleton<INotificationScheduler>(
            new QuartzNotificationScheduler(schedulerFactory, new FixedTimeProvider()));
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        _app = builder.Build();
        _app.UseExceptionHandler();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapControllers();
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        await _quartz.Shutdown();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("GET")]
    [InlineData("DELETE")]
    public async Task AnonymousRequests_ReturnUnauthorized(string method)
    {
        using var request = RequestFor(method);
        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("GET")]
    [InlineData("DELETE")]
    public async Task AuthenticatedNonAdminRequests_ReturnForbidden(string method)
    {
        Authenticate("user", "role");
        using var request = RequestFor(method);
        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("admin", "role")]
    [InlineData("Admin", "role")]
    [InlineData("admin", ClaimTypes.Role)]
    [InlineData("Admin", ClaimTypes.Role)]
    public async Task AdminCanCreateFollowLocationAndCancelSchedule(string role, string claimType)
    {
        Authenticate(role, claimType);
        var request = NewRequest();

        using var createdResponse = await _client.PostAsJsonAsync(Route, request);

        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await createdResponse.Content.ReadFromJsonAsync<ScheduledNotificationResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(request.RunAt!.Value.ToUniversalTime(), created.RunAtUtc);
        var location = createdResponse.Headers.Location;
        Assert.NotNull(location);
        Assert.EndsWith($"{Route}/{created.Id}", location.ToString());

        using var retrievedResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.OK, retrievedResponse.StatusCode);
        var retrieved = await retrievedResponse.Content.ReadFromJsonAsync<ScheduledNotificationResponse>();
        Assert.Equal(created, retrieved);

        using var cancelledResponse = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, cancelledResponse.StatusCode);
        using var missingResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
        using var missingCancelResponse = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, missingCancelResponse.StatusCode);
    }

    [Theory]
    [InlineData("2030-01-02T08:30:00")]
    [InlineData("2030-01-02T08:30:00+25:00")]
    [InlineData("not-a-timestamp")]
    public async Task InvalidRunAtJson_ReturnsBadRequest(string runAt)
    {
        Authenticate("admin", "role");
        using var content = new StringContent(
            $$"""{"title":"Prayer reminder","body":"Time to pray","topic":"faith-reminders","runAt":"{{runAt}}"}""",
            Encoding.UTF8, "application/json");

        using var response = await _client.PostAsync(Route, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(problem.Errors.Keys, key => key.Contains("runAt", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task InvalidSchedule_UsesGlobalExceptionHandlerToReturnBadRequest()
    {
        Authenticate("admin", "role");
        var request = NewRequest();
        request.CronExpression = "0 30 20 * * ?";

        using var response = await _client.PostAsJsonAsync(Route, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("runAt", problem.Detail);
        Assert.Contains("cronExpression", problem.Detail);
    }

    private void Authenticate(string role, string claimType)
    {
        _client.DefaultRequestHeaders.Add("X-Test-Role", role);
        _client.DefaultRequestHeaders.Add("X-Test-Role-Type", claimType);
    }

    private static HttpRequestMessage RequestFor(string method) => new(
        new HttpMethod(method), method == "POST" ? Route : $"{Route}/{Guid.NewGuid()}")
    {
        Content = method == "POST" ? JsonContent.Create(NewRequest()) : null
    };

    private static ScheduleNotificationRequest NewRequest() => new()
    {
        Title = "Prayer reminder",
        Body = "Time to pray",
        Topic = "faith-reminders",
        RunAt = new DateTimeOffset(2030, 1, 2, 8, 30, 0, TimeSpan.FromHours(7))
    };

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "NotificationScheduleTest";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-Role", out var role))
                return Task.FromResult(AuthenticateResult.NoResult());

            var roleType = Request.Headers["X-Test-Role-Type"].ToString();
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "test-user"), new Claim(roleType, role.ToString())],
                SchemeName);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
        }
    }
}
