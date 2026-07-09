using HireFlow.API.Authorization;
using HireFlow.Application.Features.Plans.Commands.CreatePlan;
using HireFlow.Application.Features.Plans.Commands.UpdatePlan;
using HireFlow.Application.Features.Plans.Queries.GetPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/admin/plans")]
[Authorize(Roles = Roles.SuperAdmin)]
public class PlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlans()
    {
        var result = await _mediator.Send(new GetPlansQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdatePlanCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
