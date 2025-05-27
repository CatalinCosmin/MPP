
namespace BE.Context
{
	public interface IOwnerRepository
	{
		Task AddOwnerAsync(Owner owner);
		Task DeleteOwnerAsync(int id);
		Task<List<Owner>> GetAllOwnersAsync(GetOwnersRequest request);
		Task<Owner?> GetOwnerByIdAsync(int id);
		Task UpdateOwnerAsync(Owner owner);

		Task<List<Owner>> GetOwnersWithCarsAsync(string? ownerNameFilter, string? brandFilter);
	}
}