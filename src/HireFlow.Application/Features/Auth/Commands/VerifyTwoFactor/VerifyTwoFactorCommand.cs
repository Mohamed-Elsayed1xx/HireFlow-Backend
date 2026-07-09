using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.VerifyTwoFactor;

public record VerifyTwoFactorCommand(string PendingToken, string Code) : IRequest<Result<LoginResponse>>;
