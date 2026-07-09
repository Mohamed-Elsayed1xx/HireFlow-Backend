using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetPublicJobById;

public class GetPublicJobByIdHandler : IRequestHandler<GetPublicJobByIdQuery, Result<PublicJobDto>>
{
    private readonly IJobRepository _jobRepository;

    public GetPublicJobByIdHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<Result<PublicJobDto>> Handle(GetPublicJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetActivePublicByIdAsync(request.Id);

        if (job is null)
            return Result<PublicJobDto>.Fail("NOT_FOUND", "Job not found or no longer accepting applications.");

        return Result<PublicJobDto>.Ok(job.ToPublicDto());
    }
}
