namespace HireFlow.Application.Common.DTOs;

public record JobApplicationDto(
    Guid Id,
    Guid JobId,
    string JobTitle,
    Guid CandidateId,
    string CandidateFirstName,
    string CandidateLastName,
    string CandidateEmail,
    string? CoverLetter,
    string Stage,
    string? Source,
    DateTime CreatedAt
);

public record JobApplicationSummaryDto(
    Guid Id,
    Guid CandidateId,
    string CandidateFirstName,
    string CandidateLastName,
    string CandidateEmail,
    string Stage,
    DateTime CreatedAt
);

public record StageHistoryDto(
    Guid Id,
    string? FromStage,
    string ToStage,
    string ChangedBy,
    string? Note,
    DateTime ChangedAt
);