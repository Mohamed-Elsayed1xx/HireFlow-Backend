using HireFlow.Application.Features.Analytics.Queries.GetCandidatesOverTime;
using HireFlow.Application.Features.Analytics.Queries.GetHiringFunnel;
using HireFlow.Application.Features.Analytics.Queries.GetKpis;
using HireFlow.Application.Features.Analytics.Queries.GetSourceBreakdown;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize(Roles = Roles.CompanyLeadership)]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis()
    {
        var result = await _mediator.Send(new GetKpisQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("hiring-funnel")]
    public async Task<IActionResult> GetHiringFunnel()
    {
        var result = await _mediator.Send(new GetHiringFunnelQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("candidates-over-time")]
    public async Task<IActionResult> GetCandidatesOverTime()
    {
        var result = await _mediator.Send(new GetCandidatesOverTimeQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("source-breakdown")]
    public async Task<IActionResult> GetSourceBreakdown()
    {
        var result = await _mediator.Send(new GetSourceBreakdownQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
