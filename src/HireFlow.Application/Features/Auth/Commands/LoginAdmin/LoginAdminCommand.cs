using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.LoginAdmin;

public record LoginAdminCommand(string Email, string Password)
    : IRequest<Result<AdminLoginResponse>>;