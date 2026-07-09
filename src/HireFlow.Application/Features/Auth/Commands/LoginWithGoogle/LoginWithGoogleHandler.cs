using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.LoginWithGoogle;

/// <summary>
/// Signs a candidate in with "Sign in with Google". Finds an existing
/// candidate by email, or creates one on first sign-in (same account-linking-
/// by-verified-email approach used by most consumer apps) — either way, a
/// normal HireFlow access/refresh token pair comes back, so everything
/// downstream (guards, interceptor, refresh flow) works exactly like a
/// password login.
/// </summary>
public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, Result<LoginResponse>>
{
    private const string CandidateRole = "Candidate";

    private readonly ICandidateRepository _candidateRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;

    public LoginWithGoogleHandler(
        ICandidateRepository candidateRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        IGoogleAuthService googleAuthService)
    {
        _candidateRepository = candidateRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _googleAuthService = googleAuthService;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginWithGoogleCommand request,
        CancellationToken cancellationToken)
    {
        var googleUser = await _googleAuthService.ValidateIdTokenAsync(request.Credential);

        if (googleUser is null || !googleUser.EmailVerified || string.IsNullOrWhiteSpace(googleUser.Email))
            return Result<LoginResponse>.Fail(
                "INVALID_GOOGLE_TOKEN", "Could not verify your Google account. Please try again.");

        var candidate = await _candidateRepository.GetByEmailAsync(googleUser.Email);

        if (candidate is null)
        {
            candidate = new Candidate
            {
                FirstName = string.IsNullOrWhiteSpace(googleUser.FirstName) ? "New" : googleUser.FirstName,
                LastName = string.IsNullOrWhiteSpace(googleUser.LastName) ? "User" : googleUser.LastName,
                Email = googleUser.Email,
                // Signed in via Google — this account never uses a password.
                // Hash a random value the candidate will never know, rather
                // than leaving PasswordHash empty (BCrypt.Verify throws on a
                // non-bcrypt string, so an empty hash would crash the normal
                // email/password login instead of just rejecting it cleanly).
                PasswordHash = _passwordService.Hash(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")),
            };

            await _candidateRepository.AddAsync(candidate);
        }

        var accessToken = _tokenService.GenerateAccessToken(candidate.Id, candidate.Email, CandidateRole);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshTokenEntity
        {
            CandidateId = candidate.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return Result<LoginResponse>.Ok(new LoginResponse(
            accessToken,
            refreshToken,
            candidate.Email,
            CandidateRole
        ));
    }
}
