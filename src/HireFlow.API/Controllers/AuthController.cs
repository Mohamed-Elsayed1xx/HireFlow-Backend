using HireFlow.API.Authorization;
using HireFlow.Application.Features.Auth.Commands.DisableTwoFactor;
using HireFlow.Application.Features.Auth.Commands.EnableTwoFactor;
using HireFlow.Application.Features.Auth.Commands.ForgotPassword;
using HireFlow.Application.Features.Auth.Commands.LoginAdmin;
using HireFlow.Application.Features.Auth.Commands.LoginCandidate;
using HireFlow.Application.Features.Auth.Commands.LoginCompany;
using HireFlow.Application.Features.Auth.Commands.LoginWithGoogle;
using HireFlow.Application.Features.Auth.Commands.Logout;
using HireFlow.Application.Features.Auth.Commands.RefreshToken;
using HireFlow.Application.Features.Auth.Commands.RegisterCandidate;
using HireFlow.Application.Features.Auth.Commands.RegisterCompany;
using HireFlow.Application.Features.Auth.Commands.ResetPassword;
using HireFlow.Application.Features.Auth.Commands.SetupTwoFactor;
using HireFlow.Application.Features.Auth.Commands.VerifyTwoFactor;
using HireFlow.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login/company")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login/admin")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginAdmin([FromBody] LoginAdminCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login/admin/verify-2fa")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("2fa/setup")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> SetupTwoFactor()
    {
        var result = await _mediator.Send(new SetupTwoFactorCommand());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("2fa/enable")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("2fa/disable")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login/candidate")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginCandidate([FromBody] LoginCandidateCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login/google")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginGoogle([FromBody] LoginWithGoogleCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("register/company")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("register/candidate")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return result.Success ? Ok(result) : NotFound(result);
    }
}
