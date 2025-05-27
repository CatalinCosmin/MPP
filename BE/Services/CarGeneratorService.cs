using BE.Context;
using BE.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class CarGeneratorService : BackgroundService
{
	private readonly IHubContext<EntityHub> _hubContext;
	private readonly IServiceProvider _serviceProvider;
	private Faker<Car> _carFaker;

	public CarGeneratorService(IHubContext<EntityHub> hubContext, IServiceProvider serviceProvider)
	{
		_hubContext = hubContext;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(2000, stoppingToken);

			using var scope = _serviceProvider.CreateScope();
			var carRepository = scope.ServiceProvider.GetRequiredService<ICarRepository>();
			var context = scope.ServiceProvider.GetRequiredService<DataContext>();

			var insertedOwners = await context.Owners.AsNoTracking().Select(o => o.Id).ToListAsync();

			_carFaker = new Faker<Car>()
				.RuleFor(c => c.Id, f => Guid.NewGuid())
				.RuleFor(c => c.Name, f => $"{f.Vehicle.Manufacturer()} {f.Vehicle.Model()} {Guid.NewGuid().ToString()[..8]}")
				.RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
				.RuleFor(c => c.Model, f => f.PickRandom("Sedan", "SUV", "Truck", "Coupe", "Hatchback", "Convertible", "Electric"))
				.RuleFor(c => c.Year, f => f.Date.Past(10).Year)
				.RuleFor(c => c.Color, f => f.Commerce.Color())
				.RuleFor(c => c.Price, f => f.Random.Int(10_000, 90_000))
				.RuleFor(c => c.OwnerId, f => f.PickRandom(insertedOwners));

			var car = _carFaker.Generate();

			await carRepository.AddCar(car);

			Console.WriteLine("new car: " + car.Name);

			await _hubContext.Clients.All.SendAsync("CarsChanged");
			await _hubContext.Clients.All.SendAsync("MetricsChanged");
		}
	}
}
