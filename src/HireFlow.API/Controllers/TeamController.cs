using HireFlow.Application.Features.Team.Commands.ActivateMember;
using HireFlow.Application.Features.Team.Commands.DeactivateMember;
using HireFlow.Application.Features.Team.Commands.InviteTeamMember;
using HireFlow.Application.Features.Team.Commands.UpdateMemberRole;
using HireFlow.Application.Features.Team.Queries.GetTeamMembers;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/team")]
[Authorize(Roles = Roles.CompanyAdmin)]
public class TeamController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeamMembers([FromQuery] GetTeamMembersQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> InviteMember([FromBody] InviteTeamMemberCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateMemberRoleCommand command)
    {
        var result = await _mediator.Send(command with { MemberId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _mediator.Send(new DeactivateMemberCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _mediator.Send(new ActivateMemberCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
