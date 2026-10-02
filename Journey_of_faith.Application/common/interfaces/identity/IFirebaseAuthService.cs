namespace Journey_of_faith.Application.common.interfaces;


public interface IFirebaseAuthService
{
    Task<string?> VerifyIdTokenAsync(string idToken);
}