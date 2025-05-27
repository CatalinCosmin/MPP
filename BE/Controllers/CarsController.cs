using BE.Context;
using BE.Models;
using BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CarsController : ControllerBase
{
	private readonly ILogger<CarsController> _logger;
	private readonly ICarsService _carsService;

	public CarsController(ILogger<CarsController> logger, ICarsService carsService)
	{
		_logger = logger;
		_carsService = carsService;
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] GetCarsRequest request)
	{
		var result = await _carsService.GetCars(request);

		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] AddCarRequest carRequest)
	{
		await _carsService.AddCar(carRequest);

		return Created();
	}

	[HttpDelete("{name}")]
	public async Task<IActionResult> Delete([FromRoute] string name)
	{
		await _carsService.DeleteCar(name);

		return NoContent();
	}

	[HttpPatch("{id}")]
	public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] UpdateCarRequest request)
	{
		await _carsService.UpdateCar(id, request);

		return NoContent();
	}
}
