namespace BE.Context;

public class Owner
{
	public int Id { get; set; }
	public string Name { get; set; }
	public List<Car> Cars { get; set; }
}
