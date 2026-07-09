using HireFlow.Application.Features.Companies.Commands.ActivateCompany;
using HireFlow.Application.Features.Companies.Commands.CreateCompany;
using HireFlow.Application.Features.Companies.Commands.SuspendCompany;
using HireFlow.Application.Features.Companies.Commands.UpdateCompany;
using HireFlow.Application.Features.Companies.Commands.UpdateCompanyPlan;
using HireFlow.Application.Features.Companies.Queries.GetCompanies;
using HireFlow.Application.Features.Companies.Queries.GetCompanyById;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/admin/companies")]
[Authorize(Roles = Roles.SuperAdmin)]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies([FromQuery] GetCompaniesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/suspend")]
    public async Task<IActionResult> SuspendCompany(Guid id)
    {
        var result = await _mediator.Send(new SuspendCompanyCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateCompany(Guid id)
    {
        var result = await _mediator.Send(new ActivateCompanyCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/plan")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdateCompanyPlanCommand command)
    {
        var result = await _mediator.Send(command with { CompanyId = id });
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
