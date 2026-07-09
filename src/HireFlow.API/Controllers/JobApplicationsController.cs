using HireFlow.Application.Features.JobApplications.Commands.BulkMoveApplications;
using HireFlow.Application.Features.JobApplications.Commands.RejectApplication;
using HireFlow.Application.Features.JobApplications.Commands.UpdateApplicationStage;
using HireFlow.Application.Features.JobApplications.Queries.GetApplicationById;
using HireFlow.Application.Features.JobApplications.Queries.GetApplications;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize(Roles = Roles.AnyCompanyUser)]
public class JobApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] GetApplicationsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApplicationById(Guid id)
    {
        var result = await _mediator.Send(new GetApplicationByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("{id:guid}/stage")]
    public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateApplicationStageCommand command)
    {
        var result = await _mediator.Send(command with { ApplicationId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectApplicationCommand command)
    {
        var result = await _mediator.Send(command with { ApplicationId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("bulk-move")]
    public async Task<IActionResult> BulkMove([FromBody] BulkMoveApplicationsCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
