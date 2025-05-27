// Services/AuthService.cs
using BE.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
	private readonly DataContext _context;
	private readonly string _jwtSecretKey;

	public AuthService(DataContext context, IConfiguration configuration)
	{
		_context = context;
		_jwtSecretKey = configuration["JwtSecretKey"];
	}

	public async Task RegisterAsync(string username, string password)
	{
		var user = new User
		{
			Username = username,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
			Role = "User"
		};
		_context.Users.Add(user);
		await _context.SaveChangesAsync();
	}

	public async Task<string?> LoginAsync(string username, string password)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
		if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
		{
			var token = GenerateJwtToken(user);
			return token;
		}
		return null;
	}

	private string GenerateJwtToken(User user)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Role, user.Role)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: "your-app",
			audience: "your-app",
			claims: claims,
			expires: DateTime.Now.AddHours(1),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
