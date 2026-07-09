namespace HireFlow.Domain.Interfaces.Services;

public interface IFileService
{
    /// <summary>
    /// Saves the CV and returns a server-relative URL (e.g. /uploads/cvs/{id}/{file}).
    /// </summary>
    Task<string> UploadCvAsync(Stream fileStream, string fileName, Guid candidateId);
    Task DeleteCvAsync(string relativeUrl);
}
