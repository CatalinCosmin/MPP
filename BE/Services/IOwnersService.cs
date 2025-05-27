
namespace BE.Services
{
	public interface IOwnersService
	{
		Task AddOwnerAsync(AddOwnerRequest request);
		Task DeleteOwnerAsync(int id);
		Task<GetOwnersResponse> GetOwnersAsync(GetOwnersRequest request);
		Task UpdateOwnerAsync(int id, UpdateOwnerRequest request);
	}
}