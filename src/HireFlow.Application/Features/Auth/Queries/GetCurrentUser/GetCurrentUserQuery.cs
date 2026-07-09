using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;