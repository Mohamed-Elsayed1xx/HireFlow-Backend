using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.AddJobAssignee;

public class AddJobAssigneeHandler : IRequestHandler<AddJobAssigneeCommand, Result>
{
    private readonly IJobRepository _jobRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public AddJobAssigneeHandler(
        IJobRepository jobRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AddJobAssigneeCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.JobId);

        if (job is null)
            return Result.Fail("NOT_FOUND", "Job not found.");

        if (job.CompanyId != _currentUser.CompanyId)
            return Result.Fail("FORBIDDEN", "Access denied.");

        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null || user.CompanyId != _currentUser.CompanyId)
            return Result.Fail("NOT_FOUND", "User not found.");

        var assignee = new JobAssignee
        {
            JobId = request.JobId,
            UserId = request.UserId,
            Role = request.Role
        };

        job.Assignees.Add(assignee);
        await _jobRepository.UpdateAsync(job);

        return Result.Ok("Assignee added successfully.");
    }
}