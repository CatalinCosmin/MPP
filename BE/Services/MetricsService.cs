using BE.Models;
using BE.Context;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class MetricsService : IMetricsService
{
	private readonly ICarRepository _carRepository;
	private readonly IOwnerRepository _ownerRepository;

	public MetricsService(ICarRepository carRepository, IOwnerRepository ownerRepository)
	{
		_carRepository = carRepository;
		_ownerRepository = ownerRepository;
	}
	public async Task<Metrics> CalculateMetric()
	{
		var (totalCars, totalPrice) = await _carRepository.GetCarAggregatesAsync();
		var averagePrice = totalCars > 0 ? totalPrice / totalCars : 0;

		return new Metrics
		{
			TotalCars = totalCars,
			TotalPrice = totalPrice,
			AveragePrice = averagePrice
		};
	}


	public async Task<List<OwnerCarStat>> GetCarCountPerOwnerAsync(string? ownerNameFilter = null, string? brandFilter = null)
	{
		var stats = await _ownerRepository.GetOwnersWithCarsAsync(ownerNameFilter, brandFilter);

		return stats
			.Select(o => new OwnerCarStat
			{
				OwnerId = o.Id,
				OwnerName = o.Name,
				CarCount = o.Cars.Count
			})
			.ToList();
	}

	public async Task<List<OwnerCarStat>> GetCarCountPerOwnerUnoptimizedAsync(string? ownerNameFilter = null, string? brandFilter = null)
	{
		var owners = await _ownerRepository.GetOwnersWithCarsAsync(ownerNameFilter, string.Empty);

		var filteredOwners = owners
			.Where(o => string.IsNullOrWhiteSpace(ownerNameFilter) || o.Name.Contains(ownerNameFilter, StringComparison.OrdinalIgnoreCase))
			.ToList();

		var stats = filteredOwners.Select(owner => new OwnerCarStat
		{
			OwnerId = owner.Id,
			OwnerName = owner.Name,
			CarCount = owner.Cars.Count(car => string.IsNullOrWhiteSpace(brandFilter) || car.Brand.Contains(brandFilter, StringComparison.OrdinalIgnoreCase))
		}).ToList();

		return stats;
	}
}
