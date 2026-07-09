using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class JobApplicationMappings
{
    public static JobApplicationDto ToDto(this JobApplication application) => new(
        application.Id,
        application.JobId,
        application.Job?.Title ?? string.Empty,
        application.CandidateId,
        application.Candidate?.FirstName ?? string.Empty,
        application.Candidate?.LastName ?? string.Empty,
        application.Candidate?.Email ?? string.Empty,
        application.CoverLetter,
        application.Stage.ToString(),
        application.Source,
        application.CreatedAt
    );

    public static JobApplicationSummaryDto ToSummaryDto(this JobApplication application) => new(
        application.Id,
        application.CandidateId,
        application.Candidate?.FirstName ?? string.Empty,
        application.Candidate?.LastName ?? string.Empty,
        application.Candidate?.Email ?? string.Empty,
        application.Stage.ToString(),
        application.CreatedAt
    );

    public static StageHistoryDto ToDto(this ApplicationStageHistory history) => new(
        history.Id,
        history.FromStage?.ToString(),
        history.ToStage.ToString(),
        $"{history.ChangedBy.FirstName} {history.ChangedBy.LastName}",
        history.Note,
        history.ChangedAt
    );
}