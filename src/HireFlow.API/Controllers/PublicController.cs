using HireFlow.Application.Features.Jobs.Queries.GetPublicJobById;
using HireFlow.Application.Features.Jobs.Queries.GetPublicJobs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

/// <summary>
/// Public-facing job board. No authentication required — this is what
/// anonymous visitors see before deciding to create a candidate account.
/// </summary>
[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs([FromQuery] GetPublicJobsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("jobs/{id:guid}")]
    public async Task<IActionResult> GetJobById(Guid id)
    {
        var result = await _mediator.Send(new GetPublicJobByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }
}
