using BE.Models;
using BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OwnersController : ControllerBase
{
	private readonly ILogger<OwnersController> _logger;
	private readonly IOwnersService _ownerService;

	public OwnersController(ILogger<OwnersController> logger, IOwnersService ownerService)
	{
		_logger = logger;
		_ownerService = ownerService;
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] GetOwnersRequest request)
	{
		var result = await _ownerService.GetOwnersAsync(request);
		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] AddOwnerRequest request)
	{
		await _ownerService.AddOwnerAsync(request);
		return Created();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete([FromRoute] int id)
	{
		await _ownerService.DeleteOwnerAsync(id);
		return NoContent();
	}

	[HttpPatch("{id:int}")]
	public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateOwnerRequest request)
	{
		await _ownerService.UpdateOwnerAsync(id, request);
		return NoContent();
	}
}
