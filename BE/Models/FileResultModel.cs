namespace BE.Models;

public class FileResultModel
{
	public byte[] Content { get; set; } = default!;
	public string FileName { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
}
