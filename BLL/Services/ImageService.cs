using BLL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BLL.Services;

public class ImageService : IImageService
{
    private readonly string _imageDirectory;

    public ImageService(string imageDirectory = "wwwroot/Images")
    {
        _imageDirectory = imageDirectory;
    }

    public async Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentNullException(nameof(file));
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_imageDirectory, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return $"/Images/{fileName}";
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        var fileName = imagePath.Split('/').Last();
        var fullPath = Path.Combine(_imageDirectory, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
