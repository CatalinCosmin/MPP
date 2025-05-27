using BE.Context;
using BE.Models;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class FileService : IFileService
{
	private readonly DataContext _context;

	public FileService(DataContext context)
	{
		_context = context;
	}

	public async Task<Guid> SaveFileAsync(IFormFile file)
	{
		if (file == null || file.Length == 0)
			throw new ArgumentException("File is empty");

		using var memoryStream = new MemoryStream();
		await file.CopyToAsync(memoryStream);

		var storedFile = new StoredFile
		{
			FileName = file.FileName,
			ContentType = file.ContentType,
			Data = memoryStream.ToArray()
		};

		_context.StoredFiles.Add(storedFile);
		await _context.SaveChangesAsync();

		return storedFile.Id;
	}


	public IEnumerable<StoredFile> GetAllFiles()
	{
		return _context.StoredFiles
			.Select(f => new StoredFile
			{
				Id = f.Id,
				FileName = f.FileName,
				ContentType = f.ContentType,
				UploadedAt = f.UploadedAt,
				Data = f.Data
			})
			.ToList();
	}

	public async Task<FileResultModel?> GetFileAsync(Guid id)
	{
		var file = await _context.StoredFiles.FirstOrDefaultAsync(f => f.Id == id);

		if (file == null)
			return null;

		return new FileResultModel
		{
			Content = file.Data,
			ContentType = file.ContentType,
			FileName = file.FileName
		};
	}
}
