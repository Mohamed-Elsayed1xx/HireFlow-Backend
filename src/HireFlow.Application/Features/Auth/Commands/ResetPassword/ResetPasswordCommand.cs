using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
