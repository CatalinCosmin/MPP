namespace BE.Models;

public class OwnerCarStat
{
	public int OwnerId { get; set; }
	public string OwnerName { get; set; } = string.Empty;
	public int CarCount { get; set; }
}
