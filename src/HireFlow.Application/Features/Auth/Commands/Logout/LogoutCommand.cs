using MediatR;
using HireFlow.Application.Common.Models;

namespace HireFlow.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) 
    : IRequest<Result>;