using BE.Models;
using Microsoft.EntityFrameworkCore;

namespace BE.Context;

public class CarRepository : ICarRepository
{
	private readonly DataContext _context;

	public CarRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<List<Car>> GetCars(GetCarsRequest request)
	{
		var cars = _context.Cars;

		var query = cars.AsQueryable();
		if(!string.IsNullOrWhiteSpace(request.FilterName))
			query = query.Where(car => car.Name.Contains(request.FilterName));
		if(!string.IsNullOrWhiteSpace(request.FilterBrand))
			query = query.Where(car => car.Brand.Contains(request.FilterBrand));

		if (request.SortBy == nameof(Car.Id))
			query = query.OrderBy(car => car.Id);
		else if (request.SortBy == nameof(Car.Name))
			query = query.OrderBy(car => car.Name);
		else if (request.SortBy == nameof(Car.Price))
			query = query.OrderBy(car => car.Price);

		var result = await query
			.Skip(request.Offset)
			.Take(request.Limit)
			.ToListAsync();

		return result;
	}

	public async Task AddCar(Car newCar)
	{
		var car = await GetCarByName(newCar.Name);

		if (car is not null)
		{
			throw new ArgumentException("Car with the same name already exists");
		}

		_context.Cars.Add(newCar);
		await _context.SaveChangesAsync();
	}

	public async Task RemoveCar(Guid id)
	{
		var car = await GetCarById(id);
		if (car is null)
		{
			throw new ArgumentException($"Could not find {id} car");
		}

		_context.Cars.Remove(car);
		await _context.SaveChangesAsync();
	}

	public async Task<Car?> GetCarById(Guid id)
	{
		return await _context.Cars.FirstOrDefaultAsync(car => car.Id == id);
	}

	public async Task<Car?> GetCarByName(string name)
	{
		return await _context.Cars.FirstOrDefaultAsync(car => car.Name == name);
	}

	public async Task UpdateCar(Car updatedCar)
	{
		var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == updatedCar.Id);
		if (car == null)
		{
			throw new ArgumentException("Could not find car to update.");
		}

		car.Name = updatedCar.Name;
		car.Year = updatedCar.Year;
		car.Brand = updatedCar.Brand;
		car.Model = updatedCar.Model;
		car.Price = updatedCar.Price;
		car.Color = updatedCar.Color;

		_context.Update(car);
		await _context.SaveChangesAsync();
	}

	public async Task<(int totalCars, decimal totalPrice)> GetCarAggregatesAsync()
	{
		var totalCars = await _context.Cars.CountAsync();
		var totalPrice = await _context.Cars.SumAsync(c => c.Price);
		return (totalCars, totalPrice);
	}
}
