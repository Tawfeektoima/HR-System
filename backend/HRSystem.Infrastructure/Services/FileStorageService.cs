using Microsoft.AspNetCore.Http;
using HRSystem.Core.Interfaces.Services;

namespace HRSystem.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cvs");

    public FileStorageService()
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveCvFileAsync(IFormFile file, int candidateId)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"candidate_{candidateId}_{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/cvs/{fileName}";
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        // filePath in DB is likely "/cvs/filename.pdf"
        var actualPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
        
        if (!File.Exists(actualPath))
            throw new FileNotFoundException("File not found", filePath);

        return await File.ReadAllBytesAsync(actualPath);
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var actualPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
        if (File.Exists(actualPath))
        {
            File.Delete(actualPath);
            await Task.CompletedTask;
        }
    }

    public string GetFileUrl(string filePath)
    {
        // Depends on domain, returning relative for now
        return filePath;
    }
}
