using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private const string CandidateRole = "Candidate";

    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserHandler(
        IUserRepository userRepository,
        ICandidateRepository candidateRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.Role == CandidateRole)
        {
            var candidate = await _candidateRepository.GetByIdAsync(_currentUser.UserId);

            if (candidate is null)
                return Result<UserDto>.Fail("NOT_FOUND", "User not found.");

            return Result<UserDto>.Ok(new UserDto(
                candidate.Id,
                candidate.FirstName,
                candidate.LastName,
                candidate.Email,
                CandidateRole,
                null,
                false
            ));
        }

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user is null)
            return Result<UserDto>.Fail("NOT_FOUND", "User not found.");

        return Result<UserDto>.Ok(new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString(),
            user.CompanyId,
            user.TwoFactorEnabled
        ));
    }
}