using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Commands.UploadCv;

// Frontend expects `{ cvUrl: string }`, not a bare string — see MyService.uploadCv()
// in the Angular client, which reads `res.data.cvUrl`.
public record UploadCvResult(string CvUrl);

public record UploadCvCommand(Stream FileStream, string FileName) : IRequest<Result<UploadCvResult>>;
