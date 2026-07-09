using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.UpdateMemberRole;

public class UpdateMemberRoleHandler : IRequestHandler<UpdateMemberRoleCommand, Result<TeamMemberDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateMemberRoleHandler(IUserRepository userRepository, ICurrentUserService currentUser, IAuditService auditService)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<TeamMemberDto>> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var member = await _userRepository.GetByIdAsync(request.MemberId);

        if (member is null || member.CompanyId != _currentUser.CompanyId)
            return Result<TeamMemberDto>.Fail("NOT_FOUND", "Team member not found.");

        if (member.Id == _currentUser.UserId)
            return Result<TeamMemberDto>.Fail("INVALID_OPERATION", "Cannot change your own role.");

        if (!Enum.TryParse<UserRole>(request.Role, out var role) || role == UserRole.SuperAdmin)
            return Result<TeamMemberDto>.Fail("INVALID_ROLE", "Invalid role for a company team member.");

        var oldRole = member.Role;
        member.Role = role;
        member.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(member);

        await _auditService.LogAsync("MemberRoleChanged", "User", member.Id, new { Role = oldRole.ToString() }, new { Role = role.ToString() });

        return Result<TeamMemberDto>.Ok(member.ToDto(), "Team member role updated successfully.");
    }
}
