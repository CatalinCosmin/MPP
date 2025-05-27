using BE.Context;

namespace BE.Models;

public class GetCarsResponse
{
	public List<Car> Result { get; set; }
	public int Offset { get; set; }
	public int Total { get; set; }
}
