using System.IdentityModel.Tokens.Jwt;
using HireFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HireFlow.Infrastructure.Services;

/// <summary>
/// Validates Google "Sign in with Google" ID tokens using only the packages
/// already in this project (Microsoft.IdentityModel.Tokens +
/// System.IdentityModel.Tokens.Jwt) — no Google.Apis.Auth dependency needed.
///
/// Google's public signing keys are fetched from its standard JWKS endpoint
/// and cached in memory for an hour (they rotate infrequently and Google
/// documents that clients should cache them, not fetch on every request).
/// </summary>
public class GoogleAuthService : IGoogleAuthService
{
    private const string GoogleCertsUrl = "https://www.googleapis.com/oauth2/v3/certs";
    private static readonly string[] ValidIssuers = { "https://accounts.google.com", "accounts.google.com" };

    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static JsonWebKeySet? _cachedKeySet;
    private static DateTime _cacheExpiresAt = DateTime.MinValue;

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken)
    {
        var clientId = _configuration["Google:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(idToken))
            return null;

        try
        {
            var keySet = await GetGoogleSigningKeysAsync();

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = ValidIssuers,
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                IssuerSigningKeys = keySet.Keys,
            };

            var principal = handler.ValidateToken(idToken, validationParameters, out _);

            var email = principal.FindFirst("email")?.Value;
            var emailVerified = principal.FindFirst("email_verified")?.Value == "true";
            var firstName = principal.FindFirst("given_name")?.Value;
            var lastName = principal.FindFirst("family_name")?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return new GoogleUserInfo(email, firstName, lastName, emailVerified);
        }
        catch
        {
            // Any failure (expired token, bad signature, network error fetching
            // certs, wrong audience, etc.) means "not a valid Google sign-in" —
            // the caller treats a null return as invalid credentials.
            return null;
        }
    }

    private async Task<JsonWebKeySet> GetGoogleSigningKeysAsync()
    {
        if (_cachedKeySet is not null && DateTime.UtcNow < _cacheExpiresAt)
            return _cachedKeySet;

        await CacheLock.WaitAsync();
        try
        {
            if (_cachedKeySet is not null && DateTime.UtcNow < _cacheExpiresAt)
                return _cachedKeySet;

            var client = _httpClientFactory.CreateClient();
            var json = await client.GetStringAsync(GoogleCertsUrl);

            _cachedKeySet = new JsonWebKeySet(json);
            _cacheExpiresAt = DateTime.UtcNow.AddHours(1);

            return _cachedKeySet;
        }
        finally
        {
            CacheLock.Release();
        }
    }
}
