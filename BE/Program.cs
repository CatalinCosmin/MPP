using BE.Context;
using BE.Middleware;
using BE.Models;
using BE.Services;
using BE.SignalR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:3000", "https://localhost:7092")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

builder.Services.AddDbContext<DataContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();	

builder.Services.AddScoped<ICarsService, CarsService>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IOwnersService, OwnersService>(); 


builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<HostOptions>(options =>
			options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

builder.Services.AddValidatorsFromAssemblyContaining<AddCarRequest>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHealthChecks();

builder.Services.AddSignalR();
builder.Services.AddHostedService<CarGeneratorService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IMonitoredUsersService, MonitoredUsersService>();
builder.Services.AddHostedService<MonitoringService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]))
		};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// TO DO: de adaugat test pt file, de reparat file, de conectat file la frontend
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");
app.MapHub<EntityHub>("/entityhub");

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<LogMiddleware>();

app.MapControllers();

await MigrateDatabase(app);

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<DataContext>();
	await DatabaseSeeder.SeedAsync(context);
}

app.Run();

static async Task MigrateDatabase(WebApplication app)
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<DataContext>();

	var retryCount = 0;
	const int maxRetries = 10;
	while (retryCount < maxRetries)
	{
		try
		{
			await db.Database.MigrateAsync();
			break;
		}
		catch (SqlException)
		{
			retryCount++;
			Thread.Sleep(5000);
		}
	}
}
