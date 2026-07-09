using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private const string CandidateRole = "Candidate";

    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ICandidateRepository candidateRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // The refresh token is an opaque random string, not a JWT — it must be
        // looked up against what we actually issued and stored at login time.
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            return Result<LoginResponse>.Fail("INVALID_TOKEN", "Invalid or expired refresh token.");

        string newAccessToken;
        string email;
        string role;

        if (storedToken.UserId is Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null || !user.IsActive)
                return Result<LoginResponse>.Fail("INVALID_TOKEN", "Invalid or expired refresh token.");

            email = user.Email;
            role = user.Role.ToString();
            newAccessToken = _tokenService.GenerateAccessToken(user.Id, email, role, user.CompanyId);
        }
        else if (storedToken.CandidateId is Guid candidateId)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId);

            if (candidate is null)
                return Result<LoginResponse>.Fail("INVALID_TOKEN", "Invalid or expired refresh token.");

            email = candidate.Email;
            role = CandidateRole;
            newAccessToken = _tokenService.GenerateAccessToken(candidate.Id, email, role);
        }
        else
        {
            return Result<LoginResponse>.Fail("INVALID_TOKEN", "Invalid or expired refresh token.");
        }

        // Rotate: the old refresh token is single-use, a fresh one replaces it.
        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshTokenEntity
        {
            UserId = storedToken.UserId,
            CandidateId = storedToken.CandidateId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return Result<LoginResponse>.Ok(new LoginResponse(newAccessToken, newRefreshToken, email, role));
    }
}
