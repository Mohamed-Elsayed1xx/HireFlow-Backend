using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviewById;

public record GetInterviewByIdQuery(Guid Id) : IRequest<Result<InterviewDto>>;
