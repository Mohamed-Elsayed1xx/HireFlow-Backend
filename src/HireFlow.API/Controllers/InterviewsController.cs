using HireFlow.Application.Features.Interviews.Commands.CancelInterview;
using HireFlow.Application.Features.Interviews.Commands.CreateInterview;
using HireFlow.Application.Features.Interviews.Commands.SubmitEvaluation;
using HireFlow.Application.Features.Interviews.Commands.UpdateInterview;
using HireFlow.Application.Features.Interviews.Queries.GetInterviewById;
using HireFlow.Application.Features.Interviews.Queries.GetInterviewEvaluations;
using HireFlow.Application.Features.Interviews.Queries.GetInterviews;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/interviews")]
[Authorize(Roles = Roles.AnyCompanyUser)]
public class InterviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetInterviews([FromQuery] GetInterviewsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInterviewById(Guid id)
    {
        var result = await _mediator.Send(new GetInterviewByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:guid}/evaluations")]
    public async Task<IActionResult> GetEvaluations(Guid id)
    {
        var result = await _mediator.Send(new GetInterviewEvaluationsQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInterview([FromBody] CreateInterviewCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateInterview(Guid id, [FromBody] UpdateInterviewCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelInterview(Guid id)
    {
        var result = await _mediator.Send(new CancelInterviewCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/evaluations")]
    public async Task<IActionResult> SubmitEvaluation(Guid id, [FromBody] SubmitEvaluationCommand command)
    {
        var result = await _mediator.Send(command with { InterviewId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
