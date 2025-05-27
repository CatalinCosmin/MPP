using BE.Context;
using BE.Models;
using System;

namespace BE.Services;

public class MonitoringService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	public MonitoringService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = _serviceProvider.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<DataContext>();
			var cutoff = DateTime.UtcNow.AddMinutes(-1);
			var frequentUsers = db.UserActionLogs
				.Where(log => log.Timestamp > cutoff)
				.GroupBy(log => log.UserId)
				.Where(g => g.Count() >= 100)
				.Select(g => g.Key)
				.ToList();

			foreach (var userId in frequentUsers)
			{
				db.MonitoredUsers.Add(new MonitoredUser { UserId = userId });
			}
			await db.SaveChangesAsync();
			await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
		}
	}
}