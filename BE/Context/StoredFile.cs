using System;

namespace BE.Context;

public class StoredFile
{
	public Guid Id { get; set; }
	public string FileName { get; set; } = null!;
	public string ContentType { get; set; } = null!;
	public byte[] Data { get; set; } = null!;
	public DateTime UploadedAt { get; set; }
}
