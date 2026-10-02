// using System.Security.Claims;

// namespace Journey_of_faith.Api.authorization;

// public static class NotificationSchedulingAuthorization
// {
//     public const string PolicyName = "ManageNotificationSchedules";

//     public static IServiceCollection AddNotificationSchedulingAuthorization(this IServiceCollection services)
//     {
//         services.AddAuthorization(options => options.AddPolicy(PolicyName, policy => policy
//             .RequireAuthenticatedUser()
//             .RequireAssertion(context => context.User.Claims.Any(claim =>
//                 (claim.Type == "role" || claim.Type == ClaimTypes.Role) &&
//                 string.Equals(claim.Value, "admin", StringComparison.OrdinalIgnoreCase)))));
//         return services;
//     }
// }
