using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Team.Queries.GetTeamMembers;

public class GetTeamMembersHandler : IRequestHandler<GetTeamMembersQuery, Result<PagedResult<TeamMemberDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public GetTeamMembersHandler(IUserRepository userRepository, ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<TeamMemberDto>>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<PagedResult<TeamMemberDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var members = await _userRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        if (request.Role is not null && Enum.TryParse<UserRole>(request.Role, out var role))
            members = members.Where(u => u.Role == role);

        if (request.IsActive.HasValue)
            members = members.Where(u => u.IsActive == request.IsActive.Value);

        var totalCount = members.Count();
        var items = members
            .OrderBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => u.ToDto());

        return Result<PagedResult<TeamMemberDto>>.Ok(new PagedResult<TeamMemberDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}