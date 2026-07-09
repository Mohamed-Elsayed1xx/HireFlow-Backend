namespace HireFlow.Domain.Interfaces.Services;

/// <summary>
/// Time-based One-Time Password (RFC 6238) for two-factor authentication.
/// Implemented in-house (HMAC-SHA1 + Base32, both available in the BCL)
/// rather than pulling in a third-party package for something this small.
/// </summary>
public interface ITotpService
{
    /// <summary>Generates a new random Base32-encoded secret for a user enrolling in 2FA.</summary>
    string GenerateSecret();

    /// <summary>Builds the otpauth:// URI an authenticator app can scan as a QR code.</summary>
    string BuildAuthenticatorUri(string secret, string accountEmail);

    /// <summary>Validates a 6-digit code against the secret, tolerating one step of clock drift.</summary>
    bool ValidateCode(string secret, string code);
}
