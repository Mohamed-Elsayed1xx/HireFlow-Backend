using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (token is null)
            return Result.Fail("INVALID_TOKEN", "Invalid refresh token.");

        if (token.UserId is Guid userId)
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        else if (token.CandidateId is Guid candidateId)
            await _refreshTokenRepository.RevokeAllCandidateTokensAsync(candidateId);

        return Result.Ok("Logged out successfully.");
    }
}
