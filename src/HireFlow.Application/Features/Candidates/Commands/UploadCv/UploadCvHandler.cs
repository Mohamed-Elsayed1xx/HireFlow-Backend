using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Commands.UploadCv;

public class UploadCvHandler : IRequestHandler<UploadCvCommand, Result<UploadCvResult>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IFileService _fileService;
    private readonly ICurrentUserService _currentUser;

    public UploadCvHandler(
        ICandidateRepository candidateRepository,
        IFileService fileService,
        ICurrentUserService currentUser)
    {
        _candidateRepository = candidateRepository;
        _fileService = fileService;
        _currentUser = currentUser;
    }

    public async Task<Result<UploadCvResult>> Handle(UploadCvCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetWithProfileAsync(_currentUser.UserId);

        if (candidate is null)
            return Result<UploadCvResult>.Fail("NOT_FOUND", "Profile not found.");

        // Delete old CV if exists
        if (!string.IsNullOrEmpty(candidate.Profile?.CvUrl))
            await _fileService.DeleteCvAsync(candidate.Profile.CvUrl);

        var cvUrl = await _fileService.UploadCvAsync(request.FileStream, request.FileName, candidate.Id);

        if (candidate.Profile is null)
        {
            var profile = new CandidateProfile
            {
                CandidateId = candidate.Id,
                CvUrl = cvUrl,
                Skills = []
            };
            await _candidateRepository.AddProfileAsync(profile);
            candidate.Profile = profile;
        }
        else
        {
            candidate.Profile.CvUrl = cvUrl;
            candidate.Profile.UpdatedAt = DateTime.UtcNow;
            await _candidateRepository.UpdateAsync(candidate);
        }

        return Result<UploadCvResult>.Ok(new UploadCvResult(cvUrl), "CV uploaded successfully.");
    }
}
