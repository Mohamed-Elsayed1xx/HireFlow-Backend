using FluentValidation;

namespace HireFlow.Application.Features.Interviews.Commands.CreateInterview;

public class CreateInterviewValidator : AbstractValidator<CreateInterviewCommand>
{
    public CreateInterviewValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Interview title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Interview type is required.")
            .Must(BeValidInterviewType).WithMessage("Invalid interview type.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Interview must be scheduled in the future.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.")
            .LessThanOrEqualTo(480).WithMessage("Duration cannot exceed 8 hours.");

        RuleFor(x => x.InterviewerIds)
            .NotEmpty().WithMessage("At least one interviewer must be assigned.");
    }

    private static bool BeValidInterviewType(string type)
        => new[] { "Phone", "Video", "OnSite", "Technical" }.Contains(type);
}
