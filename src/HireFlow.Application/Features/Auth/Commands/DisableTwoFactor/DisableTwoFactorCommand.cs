using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.DisableTwoFactor;

public record DisableTwoFactorCommand(string Code) : IRequest<Result>;
