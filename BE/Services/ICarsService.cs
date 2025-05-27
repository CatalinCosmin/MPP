using BE.Context;
using BE.Models;
using System.Threading.Tasks;

namespace BE.Services
{
	public interface ICarsService
	{
		Task AddCar(AddCarRequest request);
		Task DeleteCar(string name);
		Task<GetCarsResponse> GetCars(GetCarsRequest request);
		Task UpdateCar(Guid id, UpdateCarRequest request);
	}
}