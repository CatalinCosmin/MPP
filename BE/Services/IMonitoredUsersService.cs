using BE.Context;

namespace BE.Services
{
	public interface IMonitoredUsersService
	{
		Task<List<MonitoredUser>> GetMonitoredUsersAsync();
	}
}