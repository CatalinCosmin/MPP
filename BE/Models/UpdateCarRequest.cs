using FluentValidation;

namespace BE.Models;

public class UpdateCarRequest
{
	public string Name { get; set; }
	public string Brand { get; set; }
	public string Model { get; set; }
	public int Year { get; set; }
	public string Color { get; set; }
	public int Price { get; set; }
}

public class UpdateCarRequestValidator : AbstractValidator<UpdateCarRequest>
{
	public UpdateCarRequestValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty();
		RuleFor(x => x.Brand)
			.NotEmpty();
		RuleFor(x => x.Model)
			.NotEmpty();
		RuleFor(x => x.Year)
			.NotEmpty()
			.GreaterThan(0)
			.LessThan(DateTime.Now.Year);
		RuleFor(x => x.Color)
			.NotEmpty();
		RuleFor(x => x.Price)
			.NotEmpty()
			.GreaterThan(0);
	}
}
