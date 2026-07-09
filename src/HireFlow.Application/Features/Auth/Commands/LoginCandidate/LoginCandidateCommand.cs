using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.LoginCandidate;

public record LoginCandidateCommand(string Email, string Password)
    : IRequest<Result<LoginResponse>>;