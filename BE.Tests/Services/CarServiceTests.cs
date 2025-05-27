using BE.Context;
using BE.Models;
using BE.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Tests.Services;

[TestFixture]
internal class CarServiceTests
{
	private CarsService _sut;
	private Mock<ICarRepository> _carRepository;
	private List<Car> _cars;

	[SetUp]
	public void SetUp()
	{
		_carRepository = new Mock<ICarRepository>();
		_sut = new CarsService(_carRepository.Object);
		SetupCars();

		_carRepository
			.Setup(repo => repo.GetCars(It.IsAny<GetCarsRequest>()))
			.ReturnsAsync((GetCarsRequest request) =>
			{
				var filtered = string.IsNullOrWhiteSpace(request.FilterName)
					? _cars
					: _cars.Where(c => c.Name.Contains(request.FilterName)).ToList();

				return filtered
					.Skip(request.Offset)
					.Take(request.Limit)
					.ToList();
			});
	}

	[Test]
	public async Task GetCars_WhenValidRequest_ShouldReturn()
	{
		var request = new GetCarsRequest { Offset = 0, Limit = 100 };

		var result = await _sut.GetCars(request);

		result.Should().NotBeNull();
		result.Result.Should().NotBeEmpty();
		result.Result.Count.Should().Be(_cars.Count);
		result.Total.Should().Be(_cars.Count);
	}

	[Test]
	public async Task GetCars_WhenValidRequestWithPagination_ShouldReturn()
	{
		var request = new GetCarsRequest { Offset = 2, Limit = 2 };

		var result = await _sut.GetCars(request);

		result.Should().NotBeNull();
		result.Result.Should().NotBeEmpty();
		result.Result.Count.Should().Be(2);
		result.Total.Should().Be(2);
		result.Result[0].Name.Should().Be(_cars[2].Name);
	}

	[Test]
	public async Task GetCars_WhenValidRequestWithFilter_ShouldReturn()
	{
		var request = new GetCarsRequest { FilterName = "o", Offset = 0, Limit = 10 };

		var expected = _cars.Where(c => c.Name.Contains("o")).ToList();

		var result = await _sut.GetCars(request);

		result.Should().NotBeNull();
		result.Result.Should().NotBeEmpty();
		result.Result.Count.Should().Be(expected.Count);
		result.Result[0].Name.Should().Be(expected[0].Name);
	}

	[Test]
	public async Task GetCars_WhenValidRequestWithPaginationAndFilter_ShouldReturn()
	{
		var request = new GetCarsRequest { FilterName = "o", Offset = 2, Limit = 2 };

		var filtered = _cars.Where(c => c.Name.Contains("o")).Skip(2).Take(2).ToList();

		var result = await _sut.GetCars(request);

		result.Should().NotBeNull();
		result.Result.Count.Should().Be(filtered.Count);
		if (filtered.Any())
		{
			result.Result[0].Name.Should().Be(filtered[0].Name);
		}
	}

	[Test]
	public async Task AddCar_WhenValidRequest_ShouldCallRepository()
	{
		var request = new AddCarRequest
		{
			Name = "NewCar",
			Brand = "BrandX",
			Model = "ModelX",
			Year = 2023,
			Color = "Purple",
			Price = 30000
		};

		await _sut.AddCar(request);

		_carRepository.Verify(repo => repo.AddCar(It.Is<Car>(car =>
			car.Name == request.Name &&
			car.Brand == request.Brand &&
			car.Model == request.Model &&
			car.Year == request.Year &&
			car.Color == request.Color &&
			car.Price == request.Price &&
			car.Id != Guid.Empty
		)), Times.Once);
	}

	[Test]
	public async Task DeleteCar_WhenCarExists_ShouldCallRemove()
	{
		var name = "Civic";
		var existingCar = _cars.First(c => c.Name == name);

		_carRepository.Setup(r => r.GetCarByName(name)).ReturnsAsync(existingCar);

		await _sut.DeleteCar(name);

		_carRepository.Verify(r => r.RemoveCar(existingCar.Id), Times.Once);
	}

	[Test]
	public void DeleteCar_WhenCarDoesNotExist_ShouldThrow()
	{
		var name = "NonExistent";
		_carRepository.Setup(r => r.GetCarByName(name)).ReturnsAsync((Car?)null);

		Func<Task> act = async () => await _sut.DeleteCar(name);

		act.Should().ThrowAsync<ArgumentException>()
			.WithMessage($"Car with name '{name}' was not found.");
	}

	[Test]
	public async Task UpdateCar_WhenCarExists_ShouldUpdateWithNewData()
	{
		var carToUpdate = _cars.First();
		var request = new UpdateCarRequest
		{
			Name = "UpdatedName",
			Brand = "UpdatedBrand",
			Model = "UpdatedModel",
			Year = 2024,
			Color = "Yellow",
			Price = 99999
		};

		_carRepository.Setup(r => r.GetCarById(carToUpdate.Id)).ReturnsAsync(carToUpdate);

		await _sut.UpdateCar(carToUpdate.Id, request);

		_carRepository.Verify(r => r.UpdateCar(It.Is<Car>(car =>
			car.Id == carToUpdate.Id &&
			car.Name == request.Name &&
			car.Brand == request.Brand &&
			car.Model == request.Model &&
			car.Year == request.Year &&
			car.Color == request.Color &&
			car.Price == request.Price
		)), Times.Once);
	}

	[Test]
	public void UpdateCar_WhenCarDoesNotExist_ShouldThrow()
	{
		var id = Guid.NewGuid();
		var request = new UpdateCarRequest
		{
			Name = "UpdatedName",
			Brand = "UpdatedBrand",
			Model = "UpdatedModel",
			Year = 2024,
			Color = "Yellow",
			Price = 99999
		};

		_carRepository.Setup(r => r.GetCarById(id)).ReturnsAsync((Car?)null);

		Func<Task> act = async () => await _sut.UpdateCar(id, request);

		act.Should().ThrowAsync<ArgumentException>()
			.WithMessage($"Car with id '{id}' was not found.");
	}

	private void SetupCars()
	{
		_cars = new List<Car> {
			new Car { Id = Guid.NewGuid(), Name = "Corolla", Brand = "Toyota", Model = "Sedan", Year = 2020, Color = "White", Price = 20000 },
			new Car { Id = Guid.NewGuid(), Name = "Civic", Brand = "Honda", Model = "Sedan", Year = 2021, Color = "Black", Price = 22000 },
			new Car { Id = Guid.NewGuid(), Name = "Model 3", Brand = "Tesla", Model = "Electric", Year = 2022, Color = "Red", Price = 35000 },
			new Car { Id = Guid.NewGuid(), Name = "Mustang", Brand = "Ford", Model = "Coupe", Year = 2019, Color = "Blue", Price = 40000 },
			new Car { Id = Guid.NewGuid(), Name = "Camry", Brand = "Toyota", Model = "Sedan", Year = 2021, Color = "Silver", Price = 25000 },
			new Car { Id = Guid.NewGuid(), Name = "X5", Brand = "BMW", Model = "SUV", Year = 2020, Color = "Black", Price = 55000 },
			new Car { Id = Guid.NewGuid(), Name = "A4", Brand = "Audi", Model = "Sedan", Year = 2018, Color = "Gray", Price = 30000 },
			new Car { Id = Guid.NewGuid(), Name = "Wrangler", Brand = "Jeep", Model = "SUV", Year = 2022, Color = "Green", Price = 45000 },
			new Car { Id = Guid.NewGuid(), Name = "Accord", Brand = "Honda", Model = "Sedan", Year = 2020, Color = "White", Price = 27000 },
			new Car { Id = Guid.NewGuid(), Name = "Ranger", Brand = "Ford", Model = "Truck", Year = 2021, Color = "Blue", Price = 32000 }
		};
	}
}
