using BE.Context;
using BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BE.Middleware;

public class LogMiddleware
{
	private readonly RequestDelegate _next;

	public LogMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context, DataContext dbContext)
	{
		// Only log write operations (not GETs)
		if (context.User.Identity?.IsAuthenticated == true)
		{
			// Get username from token
			var username = context.User.FindFirst(ClaimTypes.Name)?.Value;

			if (!string.IsNullOrEmpty(username))
			{
				var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
				if (user != null)
				{
					dbContext.UserActionLogs.Add(new UserActionLog
					{
						UserId = user.Id,
						Action = $"{context.Request.Method} {context.Request.Path}"
					});
					await dbContext.SaveChangesAsync();
				}
			}
		}

		await _next(context);
	}
}
