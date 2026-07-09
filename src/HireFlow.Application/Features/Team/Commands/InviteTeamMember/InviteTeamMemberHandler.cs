using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.InviteTeamMember;

public class InviteTeamMemberHandler : IRequestHandler<InviteTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUser;

    public InviteTeamMemberHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IEmailService emailService,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _emailService = emailService;
        _currentUser = currentUser;
    }

    public async Task<Result<TeamMemberDto>> Handle(InviteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<TeamMemberDto>.Fail("NO_COMPANY", "User is not associated with a company.");

        if (await _userRepository.EmailExistsAsync(request.Email))
            return Result<TeamMemberDto>.Fail("EMAIL_TAKEN", "A user with this email already exists.");

        var member = new User
        {
            CompanyId = _currentUser.CompanyId.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            Role = Enum.Parse<UserRole>(request.Role),
            IsActive = true
        };

        await _userRepository.AddAsync(member);

        await _emailService.SendAsync(
            member.Email,
            "You've been invited to join HireFlow",
            $"Hi {member.FirstName}, you've been added to the team as a {member.Role}. You can now sign in using your email and the password provided by your administrator.");

        return Result<TeamMemberDto>.Ok(member.ToDto(), "Team member invited successfully.");
    }
}
