namespace GymManagementBLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        // Upload 
        Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        bool Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);


    }
}
