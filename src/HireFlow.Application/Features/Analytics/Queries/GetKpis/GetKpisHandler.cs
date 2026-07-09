using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetKpis;

public class GetKpisHandler : IRequestHandler<GetKpisQuery, Result<KpisDto>>
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public GetKpisHandler(
        IJobRepository jobRepository,
        IJobApplicationRepository applicationRepository,
        IInterviewRepository interviewRepository,
        ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _applicationRepository = applicationRepository;
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<KpisDto>> Handle(GetKpisQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<KpisDto>.Fail("NO_COMPANY", "User is not associated with a company.");

        var companyId = _currentUser.CompanyId.Value;

        var jobs = (await _jobRepository.GetByCompanyIdAsync(companyId)).ToList();
        var applications = (await _applicationRepository.GetByCompanyIdAsync(companyId)).ToList();
        var interviews = (await _interviewRepository.GetByCompanyIdAsync(companyId)).ToList();

        var now = DateTime.UtcNow;
        var hiredThisMonth = applications.Count(a =>
            a.Stage == ApplicationStage.Hired &&
            a.UpdatedAt.HasValue &&
            a.UpdatedAt.Value.Year == now.Year &&
            a.UpdatedAt.Value.Month == now.Month);

        var hiredApplications = applications
            .Where(a => a.Stage == ApplicationStage.Hired && a.UpdatedAt.HasValue)
            .Select(a => (a.UpdatedAt!.Value - a.CreatedAt).TotalDays)
            .ToList();

        var averageTimeToHire = hiredApplications.Count > 0
            ? Math.Round(hiredApplications.Average(), 1)
            : 0;

        var kpis = new KpisDto(
            TotalJobs: jobs.Count,
            ActiveJobs: jobs.Count(j => j.Status == JobStatus.Active),
            TotalCandidates: applications.Select(a => a.CandidateId).Distinct().Count(),
            TotalApplications: applications.Count,
            HiredThisMonth: hiredThisMonth,
            InterviewsScheduled: interviews.Count(i => i.Status == InterviewStatus.Scheduled),
            AverageTimeToHire: averageTimeToHire
        );

        return Result<KpisDto>.Ok(kpis);
    }
}
