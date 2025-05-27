using Microsoft.AspNetCore.Mvc;
using BE.Services;

namespace BE.Controllers
{
	[ApiController]
	[Route("auth")]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _authService;

		public AuthController(AuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			await _authService.RegisterAsync(request.Username, request.Password);
			return Ok("User registered successfully.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var token = await _authService.LoginAsync(request.Username, request.Password);
			if (token != null)
			{
				return Ok(new { Token = token });
			}
			return Unauthorized("Invalid credentials.");
		}
	}

	public class RegisterRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
