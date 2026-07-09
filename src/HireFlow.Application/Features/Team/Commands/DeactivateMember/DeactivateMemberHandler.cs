using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.DeactivateMember;

public class DeactivateMemberHandler : IRequestHandler<DeactivateMemberCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public DeactivateMemberHandler(IUserRepository userRepository, ICurrentUserService currentUser, IAuditService auditService)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(DeactivateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _userRepository.GetByIdAsync(request.MemberId);

        if (member is null || member.CompanyId != _currentUser.CompanyId)
            return Result.Fail("NOT_FOUND", "Team member not found.");

        if (member.Id == _currentUser.UserId)
            return Result.Fail("INVALID_OPERATION", "Cannot deactivate your own account.");

        if (!member.IsActive)
            return Result.Fail("ALREADY_DEACTIVATED", "Member is already deactivated.");

        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(member);

        await _auditService.LogAsync("MemberDeactivated", "User", member.Id);

        return Result.Ok("Team member deactivated successfully.");
    }
}