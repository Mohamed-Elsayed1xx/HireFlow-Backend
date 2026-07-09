using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) 
    : IRequest<Result<LoginResponse>>;