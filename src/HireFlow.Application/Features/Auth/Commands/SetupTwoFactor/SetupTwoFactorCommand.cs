using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.SetupTwoFactor;

public record SetupTwoFactorCommand : IRequest<Result<TwoFactorSetupResponse>>;
