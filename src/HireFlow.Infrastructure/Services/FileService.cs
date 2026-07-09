using HireFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _storagePath;

    public FileService(IConfiguration configuration)
    {
        _storagePath = configuration["FileStorage:Path"]!;
    }

    public async Task<string> UploadCvAsync(Stream fileStream, string fileName, Guid candidateId)
    {
        var relativeCvDir = Path.Combine("cvs", candidateId.ToString());
        var absoluteDir = Path.Combine(_storagePath, relativeCvDir);
        Directory.CreateDirectory(absoluteDir);

        var extension = Path.GetExtension(fileName);
        var savedFileName = $"{Guid.NewGuid()}{extension}";

        var absolutePath = Path.Combine(absoluteDir, savedFileName);
        await using var output = File.Create(absolutePath);
        await fileStream.CopyToAsync(output);

        // Return a server-relative URL that can be served as a static file
        return $"/uploads/cvs/{candidateId}/{savedFileName}";
    }

    public Task DeleteCvAsync(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl)) return Task.CompletedTask;

        // Convert relative URL "/uploads/cvs/..." to absolute file path
        var relativePath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        // relativeUrl starts with "uploads/", storage path already points to App_Data/uploads
        // so we need to resolve from the storage path's parent
        var storageParent = Path.GetDirectoryName(_storagePath) ?? _storagePath;
        var absolutePath = Path.Combine(storageParent, relativePath);

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);

        return Task.CompletedTask;
    }
}
