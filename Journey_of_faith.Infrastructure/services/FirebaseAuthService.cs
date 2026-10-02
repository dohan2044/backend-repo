using FirebaseAdmin.Auth;
using Journey_of_faith.Application.common.interfaces;
using Microsoft.Extensions.Logging;

namespace Journey_of_faith.Infrastructure.services;
#nullable disable
public class FirebaseAuthService(ILogger<FirebaseAuthService> logger) : IFirebaseAuthService
{
    public async Task<string?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            logger.LogInformation("Firebase identity token verified successfully");
            return decodedToken.Uid;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Firebase identity token verification failed");
            return null;
        }
    }
}
