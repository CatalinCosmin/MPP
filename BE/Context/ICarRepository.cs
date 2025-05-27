
using BE.Models;

namespace BE.Context;

public interface ICarRepository
{
	Task AddCar(Car newCar);
	Task<(int totalCars, decimal totalPrice)> GetCarAggregatesAsync();
	Task<Car?> GetCarById(Guid id);
	Task<Car?> GetCarByName(string name);
	Task<List<Car>> GetCars(GetCarsRequest request);
	Task RemoveCar(Guid id);
	Task UpdateCar(Car updatedCar);
}