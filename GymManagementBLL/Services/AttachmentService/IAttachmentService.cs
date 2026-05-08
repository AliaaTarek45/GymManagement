using Microsoft.AspNetCore.Http;

namespace GymManagementBLL.Services.AttachmentService
{
	public interface IAttachmentService
	{
        // Upload 
        Task<string?> UploadAsync(IFormFile file, string folderName, CancellationToken ct = default);
        bool Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);


    }
}
