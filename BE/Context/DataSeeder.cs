using BE.Context;
using Bogus;
using Microsoft.EntityFrameworkCore;

public static class DatabaseSeeder
{
	public static async Task SeedAsync(DataContext context)
	{
		if (context.Cars.Any()) return;
		var originalOwners = new List<Owner>();
		if (context.Owners.Any())
		{
			originalOwners = await context.Owners.ToListAsync();

			context.RemoveRange(originalOwners);
			await context.SaveChangesAsync();
		}

		var ownerFaker = new Faker<Owner>()
			.RuleFor(o => o.Name, f => f.Name.FullName());

		var owners = ownerFaker.Generate(10_000);
		await context.Owners.AddRangeAsync(owners);
		await context.SaveChangesAsync();

		var insertedOwners = await context.Owners.AsNoTracking().Select(o => o.Id).ToListAsync();

		var carFaker = new Faker<Car>()
			.RuleFor(c => c.Id, f => Guid.NewGuid())
			.RuleFor(c => c.Name, f => f.Vehicle.Model())
			.RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
			.RuleFor(c => c.Model, f => f.PickRandom("Sedan", "SUV", "Truck", "Coupe", "Electric", "Convertible"))
			.RuleFor(c => c.Year, f => f.Date.Past(10).Year)
			.RuleFor(c => c.Color, f => f.Commerce.Color())
			.RuleFor(c => c.Price, f => f.Random.Int(15_000, 80_000))
			.RuleFor(c => c.OwnerId, f => f.PickRandom(insertedOwners));

		var cars = carFaker.Generate(10_000);
		await context.Cars.AddRangeAsync(cars);
		await context.SaveChangesAsync();
	}

}
