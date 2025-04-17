using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
}

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        try
        {
            // Create uploads directory if it doesn't exist
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads", "Books");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique filename
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Copy the file to the uploads folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path to be stored in the database
            return $"/Uploads/books/{fileName}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
            return null;
        }
    }
}