using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Context;
using BE.Models;
using BE.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BE.Tests.Services
{
	[TestFixture]
	public class MetricsServiceTests
	{
		private MetricsService _sut;
		private Mock<ICarRepository> _carRepository;
		private Mock<IOwnerRepository> _ownerRepository;

		[SetUp]
		public void SetUp()
		{
			_carRepository = new Mock<ICarRepository>();
			_ownerRepository = new Mock<IOwnerRepository>();
			_sut = new MetricsService(_carRepository.Object, _ownerRepository.Object);
		}

		[Test]
		public async Task CalculateMetric_WhenCarsExist_ShouldReturnCorrectMetrics()
		{
			_carRepository.Setup(r => r.GetCarAggregatesAsync()).ReturnsAsync((3, 60000));

			var result = await _sut.CalculateMetric();

			result.TotalCars.Should().Be(3);
			result.TotalPrice.Should().Be(60000);
			result.AveragePrice.Should().Be(20000);
		}

		[Test]
		public async Task CalculateMetric_WhenNoCars_ShouldReturnZeroMetrics()
		{
			_carRepository.Setup(r => r.GetCarAggregatesAsync()).ReturnsAsync((0, 0));

			var result = await _sut.CalculateMetric();

			result.TotalCars.Should().Be(0);
			result.TotalPrice.Should().Be(0);
			result.AveragePrice.Should().Be(0);
		}

		[Test]
		public async Task GetCarCountPerOwnerAsync_ShouldReturnCorrectCounts()
		{
			var owners = new List<Owner>
			{
				new Owner { Id = 1, Name = "John", Cars = new List<Car> { new(), new() } },
				new Owner { Id = 2, Name = "Jane", Cars = new List<Car> { new() } }
			};

			_ownerRepository.Setup(r => r.GetOwnersWithCarsAsync(null, null)).ReturnsAsync(owners);

			var result = await _sut.GetCarCountPerOwnerAsync();

			result.Should().HaveCount(2);
			result[0].CarCount.Should().Be(2);
			result[1].CarCount.Should().Be(1);
		}

		//[Test]
		//public async Task GetCarCountPerOwnerUnoptimizedAsync_ShouldFilterCorrectly()
		//{
		//	var owners = new List<Owner>
		//	{
		//		new Owner { Id = 1, Name = "John", Cars = new List<Car> { new Car { Brand = "Toyota" }, new Car { Brand = "Honda" } } },
		//		new Owner { Id = 2, Name = "Jane", Cars = new List<Car> { new Car { Brand = "Ford" } } }
		//	};

		//	_ownerRepository.Setup(r => r.GetAllOwnersAsync()).ReturnsAsync(owners);

		//	var result = await _sut.GetCarCountPerOwnerUnoptimizedAsync("John", "Toyota");

		//	result.Should().HaveCount(1);
		//	result[0].OwnerName.Should().Be("John");
		//	result[0].CarCount.Should().Be(1);
		//}
	}
}
