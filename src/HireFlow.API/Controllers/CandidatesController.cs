using HireFlow.Application.Features.Candidates.Queries.GetCandidateById;
using HireFlow.Application.Features.Candidates.Queries.GetCandidates;
using HireFlow.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/candidates")]
[Authorize(Roles = Roles.AnyCompanyUser)]
public class CandidatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly string _storageRoot;

    public CandidatesController(IMediator mediator, IConfiguration configuration, IWebHostEnvironment env)
    {
        _mediator = mediator;
        // "FileStorage:Path" in appsettings.json is "App_Data/uploads" — read
        // it from config instead of assuming "App_Data" (that assumption
        // silently dropped the "uploads" segment and made every download
        // 404 even when the file existed on disk).
        _storageRoot = Path.Combine(env.ContentRootPath, configuration["FileStorage:Path"] ?? "App_Data/uploads");
    }

    [HttpGet]
    public async Task<IActionResult> GetCandidates([FromQuery] GetCandidatesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCandidateById(Guid id)
    {
        var result = await _mediator.Send(new GetCandidateByIdQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Streams a candidate's CV to an authenticated, authorized company user.
    ///
    /// The static /uploads/** file server (see Program.cs) has no access
    /// control at all — any client with the URL, authenticated or not, can
    /// download the file, and CV links (resumes contain PII) were being
    /// opened directly from the frontend via that path. This endpoint
    /// replaces that for company-portal viewing: it goes through
    /// GetCandidateByIdQuery, which already enforces the real authorization
    /// rule (the candidate must have an application within the current
    /// user's company) — access denied returns 404/403 the same way viewing
    /// the candidate's profile does, and the /uploads/** static path can
    /// eventually be removed once every caller is migrated to endpoints like
    /// this one.
    /// </summary>
    [HttpGet("{id:guid}/cv")]
    public async Task<IActionResult> DownloadCv(Guid id)
    {
        var result = await _mediator.Send(new GetCandidateByIdQuery(id));
        if (!result.Success) return NotFound(result);

        var cvUrl = result.Data?.Profile?.CvUrl;
        if (string.IsNullOrEmpty(cvUrl)) return NotFound("No CV uploaded.");

        // cvUrl is stored as "/uploads/cvs/{candidateId}/{filename}", where
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
}
