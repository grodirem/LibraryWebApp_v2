using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    void DeleteImage(string imagePath);
}
