using BE.Context;
using BE.SignalR;
using Bogus;
using Microsoft.AspNetCore.SignalR;

public class CarGeneratorService
{
	private readonly IHubContext<EntityHub> _hubContext;
	private readonly IServiceProvider _serviceProvider;
	private CancellationTokenSource _cts;
	private Task _runningTask;

	public CarGeneratorService(IHubContext<EntityHub> hubContext, IServiceProvider serviceProvider)
	{
		_hubContext = hubContext;
		_serviceProvider = serviceProvider;
	}

	public void Start()
	{
		if (_runningTask != null && !_runningTask.IsCompleted)
			return; // already running

		_cts = new CancellationTokenSource();
		_runningTask = Task.Run(() => GenerateCars(_cts.Token));
	}

	public void Stop()
	{
		_cts?.Cancel();
	}

	private async Task GenerateCars(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			await Task.Delay(2000, token);

			using var scope = _serviceProvider.CreateScope();
			var carRepository = scope.ServiceProvider.GetRequiredService<ICarRepository>();
			var context = scope.ServiceProvider.GetRequiredService<DataContext>();

			var insertedOwners = await context.Owners.AsNoTracking().Select(o => o.Id).ToListAsync(token);

			var carFaker = new Faker<Car>()
				.RuleFor(c => c.Id, f => Guid.NewGuid())
				.RuleFor(c => c.Name, f => $"{f.Vehicle.Manufacturer()} {f.Vehicle.Model()} {Guid.NewGuid().ToString()[..8]}")
				.RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
				.RuleFor(c => c.Model, f => f.PickRandom("Sedan", "SUV", "Truck", "Coupe", "Hatchback", "Convertible", "Electric"))
				.RuleFor(c => c.Year, f => f.Date.Past(10).Year)
				.RuleFor(c => c.Color, f => f.Commerce.Color())
				.RuleFor(c => c.Price, f => f.Random.Int(10_000, 90_000))
				.RuleFor(c => c.OwnerId, f => f.PickRandom(insertedOwners));

			var car = carFaker.Generate();
			await carRepository.AddCar(car);

			Console.WriteLine("new car: " + car.Name);

			await _hubContext.Clients.All.SendAsync("CarsChanged");
			await _hubContext.Clients.All.SendAsync("MetricsChanged");
		}
	}
}
