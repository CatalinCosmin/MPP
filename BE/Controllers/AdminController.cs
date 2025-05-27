using BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BE.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
	private readonly IMonitoredUsersService _monitoredUsersService;

	public AdminController(IMonitoredUsersService monitoredUsersService)
	{
		_monitoredUsersService = monitoredUsersService;
	}

	[HttpGet("monitored-users")]
	public async Task<IActionResult> GetMonitoredUsers()
	{
		var users = await _monitoredUsersService.GetMonitoredUsersAsync();

		return Ok(users);
	}
}