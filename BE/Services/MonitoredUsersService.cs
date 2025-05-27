using BE.Context;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class MonitoredUsersService : IMonitoredUsersService
{
	private readonly DataContext _dataContext;

	public MonitoredUsersService(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	public async Task<List<MonitoredUser>> GetMonitoredUsersAsync()
	{
		return await _dataContext.MonitoredUsers.ToListAsync();
	}
}
