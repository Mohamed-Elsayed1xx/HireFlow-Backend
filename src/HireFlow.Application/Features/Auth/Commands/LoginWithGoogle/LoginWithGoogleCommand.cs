using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.LoginWithGoogle;

/// <summary>Credential is the ID token ("credential" field) Google Identity
/// Services hands back to the frontend after the user picks their Google
/// account.</summary>
public record LoginWithGoogleCommand(string Credential) : IRequest<Result<LoginResponse>>;
