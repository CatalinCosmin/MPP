namespace BE.Context;

public class MonitoredUser
{
	public int Id { get; set; }
	public Guid UserId { get; set; }
	public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}