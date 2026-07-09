using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.ActivateMember;

public class ActivateMemberHandler : IRequestHandler<ActivateMemberCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public ActivateMemberHandler(IUserRepository userRepository, ICurrentUserService currentUser, IAuditService auditService)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(ActivateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _userRepository.GetByIdAsync(request.MemberId);

        if (member is null || member.CompanyId != _currentUser.CompanyId)
            return Result.Fail("NOT_FOUND", "Team member not found.");

        if (member.IsActive)
            return Result.Fail("ALREADY_ACTIVE", "Member is already active.");

        member.IsActive = true;
        member.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(member);

        await _auditService.LogAsync("MemberActivated", "User", member.Id);

        return Result.Ok("Team member reactivated successfully.");
    }
}
