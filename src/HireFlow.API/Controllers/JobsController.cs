using HireFlow.Application.Features.Jobs.Commands.AddJobAssignee;
using HireFlow.Application.Features.Jobs.Commands.CreateJob;
using HireFlow.Application.Features.Jobs.Commands.DeleteJob;
using HireFlow.Application.Features.Jobs.Commands.UpdateJob;
using HireFlow.Application.Features.Jobs.Commands.UpdateJobStatus;
using HireFlow.Application.Features.Jobs.Queries.GetJobById;
using HireFlow.Application.Features.Jobs.Queries.GetJobs;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = Roles.AnyCompanyUser)]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] GetJobsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetJobById(Guid id)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.CompanyLeadership)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.CompanyLeadership)]
    public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.CompanyLeadership)]
    public async Task<IActionResult> UpdateJobStatus(Guid id, [FromBody] UpdateJobStatusCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.CompanyLeadership)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var result = await _mediator.Send(new DeleteJobCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/assignees")]
    [Authorize(Roles = Roles.CompanyLeadership)]
    public async Task<IActionResult> AddAssignee(Guid id, [FromBody] AddJobAssigneeCommand command)
    {
        var result = await _mediator.Send(command with { JobId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
