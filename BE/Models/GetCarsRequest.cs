using BE.Context;
using FluentValidation;

namespace BE.Models;

public class GetCarsRequest
{
	public string FilterName { get; set; } = string.Empty;
	public string FilterBrand { get; set; }	= string.Empty;
	public string SortBy { get; set; } = string.Empty;
	public int Offset { get; set; }
	public int Limit { get; set; }
}

public class GetCarsRequestValidator : AbstractValidator<GetCarsRequest>
{
	public GetCarsRequestValidator()
	{
		RuleFor(x => x.SortBy)
			.Matches($"(|{nameof(Car.Id)}|{nameof(Car.Name)}|{nameof(Car.Price)})");
		RuleFor(x => x.Offset)
			.GreaterThanOrEqualTo(0);
		RuleFor(x => x.Limit)
			.NotEmpty()
			.GreaterThan(0);
	}
}