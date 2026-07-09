using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.LoginCandidate;

public class LoginCandidateHandler : IRequestHandler<LoginCandidateCommand, Result<LoginResponse>>
{
    private const string CandidateRole = "Candidate";

    private readonly ICandidateRepository _candidateRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public LoginCandidateHandler(
        ICandidateRepository candidateRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _candidateRepository = candidateRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCandidateCommand request,
        CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByEmailAsync(request.Email);

        if (candidate is null)
            return Result<LoginResponse>.Fail("INVALID_CREDENTIALS", "Invalid email or password.");

        if (!_passwordService.Verify(request.Password, candidate.PasswordHash))
            return Result<LoginResponse>.Fail("INVALID_CREDENTIALS", "Invalid email or password.");

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
