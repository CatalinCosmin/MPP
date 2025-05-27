namespace BE.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GeneratorController : ControllerBase
{
	private readonly CarGeneratorService _carGeneratorService;

	public GeneratorController(CarGeneratorService carGeneratorService)
	{
		_carGeneratorService = carGeneratorService;
	}

	[HttpPost("start")]
	public IActionResult StartGenerator()
	{
		_carGeneratorService.Start();
		return Ok("Car generator started.");
	}

	[HttpPost("stop")]
	public IActionResult StopGenerator()
	{
		_carGeneratorService.Stop();
		return Ok("Car generator stopped.");
	}
}
