namespace HireFlow.Application.Common.DTOs;

public record KpisDto(
    int TotalJobs,
    int ActiveJobs,
    int TotalCandidates,
    int TotalApplications,
    int HiredThisMonth,
    int InterviewsScheduled,
    double AverageTimeToHire
);

public record HiringFunnelDto(
    string Stage,
    int Count,
    double Percentage
);

public record CandidatesOverTimeDto(
    string Month,
    int Count
);

public record SourceBreakdownDto(
    string Source,
    int Count,
    double Percentage
);