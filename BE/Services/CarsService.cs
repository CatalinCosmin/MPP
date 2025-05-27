using BE.Context;
using BE.Models;
using System.Threading.Tasks;

namespace BE.Services;

public class CarsService : ICarsService
{
	private readonly ICarRepository _carRepository;

	public CarsService(ICarRepository carRepository)
	{
		_carRepository = carRepository;
	}

	public async Task<GetCarsResponse> GetCars(GetCarsRequest request)
	{
		var cars = await _carRepository.GetCars(request);

		return new GetCarsResponse
		{
			Result = cars,
			Offset = request.Offset,
			Total = cars.Count
		};
	}

	public async Task AddCar(AddCarRequest request)
	{
		var newCar = request.ToEntity();
		newCar.Id = Guid.NewGuid();

		await _carRepository.AddCar(newCar);
	}

	public async Task DeleteCar(string name)
	{
		var car = await _carRepository.GetCarByName(name);
		if (car is null)
		{
			throw new ArgumentException($"Car with name '{name}' was not found.");
		}

		await _carRepository.RemoveCar(car.Id);
	}

	public async Task UpdateCar(Guid id, UpdateCarRequest request)
	{
		var car = await _carRepository.GetCarById(id);
		if(car is null)
		{
			throw new ArgumentException($"Car with id '{id}' was not found.");
		}

		await _carRepository.UpdateCar(new Car
		{
			Id = car.Id,
			Name = request.Name,
			Brand = request.Brand,
			Model = request.Model,
			Year = request.Year,
			Color = request.Color,
			Price = request.Price
		});
	}
}
