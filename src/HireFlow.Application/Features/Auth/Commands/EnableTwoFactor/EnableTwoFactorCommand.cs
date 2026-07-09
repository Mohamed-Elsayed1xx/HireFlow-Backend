using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.EnableTwoFactor;

public record EnableTwoFactorCommand(string Code) : IRequest<Result>;
