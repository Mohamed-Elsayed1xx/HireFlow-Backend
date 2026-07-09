using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.RegisterCandidate;

public record RegisterCandidateCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;