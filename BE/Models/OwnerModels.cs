using BE.Context;

public class GetOwnersRequest
{
	public string? NameFilter { get; set; }
	public int Offset { get; set; } = 0;
	public int Limit { get; set; } = 10;
}

public class GetOwnersResponse
{
	public List<Owner> Result { get; set; }
	public int Offset { get; set; }
	public int Total { get; set; }
}

public class AddOwnerRequest
{
	public string Name { get; set; }
}

public class UpdateOwnerRequest
{
	public string Name { get; set; }
}
