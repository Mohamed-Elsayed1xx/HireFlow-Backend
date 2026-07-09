using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.RegisterCandidate;

public class RegisterCandidateHandler : IRequestHandler<RegisterCandidateCommand, Result<LoginResponse>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public RegisterCandidateHandler(
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
        RegisterCandidateCommand request,
        CancellationToken cancellationToken)
    {
        if (await _candidateRepository.EmailExistsAsync(request.Email))
            return Result<LoginResponse>.Fail("EMAIL_EXISTS", "Email is already in use.");

        var candidate = new Candidate
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password)
        };

        await _candidateRepository.AddAsync(candidate);

        var accessToken = _tokenService.GenerateAccessToken(candidate.Id, candidate.Email, "Candidate");
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
            "Candidate"
        ));
    }
}