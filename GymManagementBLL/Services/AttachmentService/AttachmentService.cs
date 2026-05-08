using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GymManagementBLL.Services.AttachmentService
{
	public class AttachmentService : IAttachmentService
	{
		private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
		private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
      
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment env , ILogger<AttachmentService> logger)

		{
			_env = env;
            _logger = logger;

        }
        public bool Delete(string fileName, string folderName)
		{
			
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, folderName, fileName);

                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
			catch(Exception ex)
			{
                _logger.LogError(ex, "Failed to delete attachment {File}.", fileName);
                return false;
            }
		}

        public async Task<string?> UploadAsync(IFormFile file, string folderName, CancellationToken ct = default)
        {
           
                if (file is null || file.Length == 0) return null;
                if (file.Length > _maxFileSize)
                {
                    _logger.LogWarning("Rejected upload: file too large ({Size} bytes).", file.Length);
                return null;
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Rejected upload: extension {Ext} not allowed.", extension);
                return null;
            }


            var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);


            var FileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, FileName);
            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await file.CopyToAsync(fs, ct);
                return FileName;
            }
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to upload file: {ex}");
				return null;
			}
		}

        public (Stream Stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return null;

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

            if (!File.Exists(fullPath)) return null;

            var contentType = Path.GetExtension(fullPath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, contentType);
        }
    }
}
