namespace BE.Context;

public class UserActionLog
{
	public int Id { get; set; }
	public Guid UserId { get; set; }
	public string Action { get; set; } // e.g. "Created Car"
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}