
namespace BE.Context;

public class Car
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Brand { get; set; }
	public string Model { get; set; }
	public int Year { get; set; }
	public string Color { get; set; }
	public int Price { get; set; }

	public Owner Owner { get; set; }
	public int OwnerId { get; set; }
}
