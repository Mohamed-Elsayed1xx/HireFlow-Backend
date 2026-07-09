using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.DeleteJob;

public class DeleteJobHandler : IRequestHandler<DeleteJobCommand, Result>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public DeleteJobHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id);

        if (job is null)
            return Result.Fail("NOT_FOUND", "Job not found.");

        if (job.CompanyId != _currentUser.CompanyId)
            return Result.Fail("FORBIDDEN", "Access denied.");

        await _jobRepository.DeleteAsync(job);

        return Result.Ok("Job deleted successfully.");
    }
}