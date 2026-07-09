using HireFlow.API.Authorization;
using HireFlow.Application.Features.Candidates.Commands.UpdateCandidateProfile;
using HireFlow.Application.Features.Candidates.Commands.UploadCv;
using HireFlow.Application.Features.Candidates.Queries.GetMyProfile;
using HireFlow.Application.Features.JobApplications.Commands.CreateApplication;
using HireFlow.Application.Features.JobApplications.Queries.GetMyApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HireFlow.API.Controllers;

/// <summary>
/// Self-service area for the signed-in candidate: their own profile,
/// CV, and the applications they have submitted.
/// </summary>
[ApiController]
[Route("api/my")]
[Authorize(Roles = Roles.Candidate)]
public class MyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly string _storageRoot;

    public MyController(IMediator mediator, IConfiguration configuration, IWebHostEnvironment env)
    {
        _mediator = mediator;
        // "FileStorage:Path" in appsettings.json is "App_Data/uploads" — read
        // it from config instead of hardcoding "App_Data" here, since that
        // hardcoded value silently dropped the "uploads" segment and made
        // every CV download 404 even when the file existed on disk.
        _storageRoot = Path.Combine(env.ContentRootPath, configuration["FileStorage:Path"] ?? "App_Data/uploads");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetMyProfileQuery());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("profile/cv")]
    public async Task<IActionResult> UploadCv(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("A non-empty file is required.");

        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadCvCommand(stream, file.FileName));

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("profile/cv")]
    public async Task<IActionResult> DownloadCv()
    {
        var profileResult = await _mediator.Send(new GetMyProfileQuery());
        if (!profileResult.Success) return NotFound();

        var cvUrl = profileResult.Data?.Profile?.CvUrl;
        if (string.IsNullOrEmpty(cvUrl)) return NotFound("No CV uploaded.");

        // CvUrl is stored as "/uploads/cvs/{candidateId}/{filename}", where
        // "uploads" is just the URL prefix — the actual files live under
        // whatever "FileStorage:Path" points to (_storageRoot already
        // includes that "uploads" folder), so we only need the part of the
        // URL *after* "/uploads/".
        var relativePath = cvUrl.TrimStart('/');
        if (relativePath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath["uploads/".Length..];
        var physicalPath = Path.Combine(_storageRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        var fullStorageRoot = Path.GetFullPath(_storageRoot);
        var fullPhysicalPath = Path.GetFullPath(physicalPath);
        if (!fullPhysicalPath.StartsWith(fullStorageRoot, StringComparison.OrdinalIgnoreCase))
            return NotFound("CV file not found.");

        if (!System.IO.File.Exists(physicalPath)) return NotFound("CV file not found.");

        var fileName = Path.GetFileName(physicalPath);
        return PhysicalFile(physicalPath, "application/octet-stream", fileName);
    }

    [HttpGet("applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var result = await _mediator.Send(new GetMyApplicationsQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("applications")]
    public async Task<IActionResult> Apply([FromBody] CreateApplicationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
