using BE.Context;

namespace BE.Models;

public static class CarExtensions
{
	public static Car ToEntity(this AddCarRequest request)
	{
		return new Car
		{
			Brand = request.Brand,
			Model = request.Model,
			Name = request.Name,
			Price = request.Price,
			Year = request.Year,
			Color = request.Color
		};
	}
}
