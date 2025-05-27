using BE.Context;
using BE.Models;

public interface IFileService
{
	IEnumerable<StoredFile> GetAllFiles();
	Task<Guid> SaveFileAsync(IFormFile file);
	Task<FileResultModel?> GetFileAsync(Guid id); // Added for download
}
