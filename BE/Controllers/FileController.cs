using BE.Context;
using BE.Models;
using BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
	private readonly IFileService _fileService;

	public FileController(IFileService fileService)
	{
		_fileService = fileService;
	}

	/// <summary>
	/// Returns all stored file metadata.
	/// </summary>
	[HttpGet]
	public ActionResult<IEnumerable<StoredFile>> GetFiles()
	{
		var files = _fileService.GetAllFiles();
		return Ok(files);
	}

	/// <summary>
	/// Uploads a file to the server.
	/// </summary>
	/// <param name="file">The file to upload</param>
	/// <returns>ID of uploaded file</returns>
	[HttpPost("upload")]
	[Consumes("multipart/form-data")] // ✅ Ensures Swagger shows file upload correctly
	public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest file)
	{
		if (file == null || file.File.Length == 0)
			return BadRequest("No file provided or file is empty.");

		var id = await _fileService.SaveFileAsync(file.File);
		return Ok(new { FileId = id });
	}


	/// <summary>
	/// Downloads a file by its ID.
	/// </summary>
	[HttpGet("download/{id}")]
	public async Task<IActionResult> DownloadFile(Guid id)
	{
		var fileResult = await _fileService.GetFileAsync(id);

		if (fileResult == null)
			return NotFound("File not found.");

		return File(fileResult.Content, fileResult.ContentType, fileResult.FileName);
	}
}
