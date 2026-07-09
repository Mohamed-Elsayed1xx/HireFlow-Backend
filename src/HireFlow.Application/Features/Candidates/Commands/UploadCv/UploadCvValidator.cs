using FluentValidation;

namespace HireFlow.Application.Features.Candidates.Commands.UploadCv;

public class UploadCvValidator : AbstractValidator<UploadCvCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadCvValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .Must(HaveAllowedExtension).WithMessage("Only PDF, DOC, and DOCX files are allowed.");

        RuleFor(x => x.FileStream.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("CV file cannot exceed 5 MB.");
    }

    private static bool HaveAllowedExtension(string fileName)
        => AllowedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant());
}
