using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.LoginCompany;

public record LoginCompanyCommand(string Email, string Password)
    : IRequest<Result<LoginResponse>>;