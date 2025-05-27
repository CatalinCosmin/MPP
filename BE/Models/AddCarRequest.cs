using FluentValidation;

namespace BE.Models;

public class AddCarRequest
{
	public string Name { get; set; }
	public string Brand { get; set; }
	public string Model { get; set; }
	public int Year { get; set; }
	public string Color { get; set; }
	public int Price { get; set; }
}

public class AddCarRequestValidator: AbstractValidator<AddCarRequest>
{
	public AddCarRequestValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty();
		RuleFor(x => x.Brand)
			.NotEmpty();
		RuleFor(x => x.Model)
			.NotEmpty();
		RuleFor(x => x.Year)
			.NotEmpty()
			.GreaterThanOrEqualTo(1900)
			.LessThanOrEqualTo(DateTime.Now.Year);
		RuleFor(x => x.Color)
			.NotEmpty();
		RuleFor(x => x.Price)
			.NotEmpty()
			.GreaterThan(0);
	}
}
