namespace HireFlow.Domain.Interfaces.Services;

/// <summary>Minimal identity claims extracted from a verified Google ID token.</summary>
public record GoogleUserInfo(string Email, string? FirstName, string? LastName, bool EmailVerified);

public interface IGoogleAuthService
{
    /// <summary>
    /// Verifies a Google "Sign in with Google" ID token (the `credential` the
    /// frontend receives from Google Identity Services) against Google's
    /// public signing keys. Returns null if the token is missing, expired,
    /// signed by the wrong key, or issued for a different Client ID.
    /// </summary>
    Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken);
}
