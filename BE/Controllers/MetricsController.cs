using BE.Models;
using BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MetricsController : ControllerBase
{
	private readonly IMetricsService _metricsService;

	public MetricsController(IMetricsService metricsService)
	{
		_metricsService = metricsService;
	}

	[HttpGet]
	public async Task<IActionResult> GetMetrics()
	{
		var metrics = await _metricsService.CalculateMetric();

		return Ok(metrics);
	}

	[HttpGet("car-count-per-owner")]
	public async Task<IActionResult> GetCarCountPerOwnerAsync([FromQuery] string? ownerNameFilter, [FromQuery] string? brandFilter)
	{
		var result = await _metricsService.GetCarCountPerOwnerAsync(ownerNameFilter, brandFilter);

		return Ok(result);
	}

	[HttpGet("car-count-per-owner-unoptimized")]
	public async Task<IActionResult> GetCarCountPerOwnerUnoptimized([FromQuery] string? ownerNameFilter, [FromQuery] string? brandFilter)
	{
		var stats = await _metricsService.GetCarCountPerOwnerUnoptimizedAsync(ownerNameFilter, brandFilter);
		return Ok(stats);
	}
}
