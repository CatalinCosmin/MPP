using BE.Context;
using BE.Models;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class OwnersService : IOwnersService
{
	private readonly IOwnerRepository _ownerRepository;

	public OwnersService(IOwnerRepository ownerRepository)
	{
		_ownerRepository = ownerRepository;
	}

	public async Task<GetOwnersResponse> GetOwnersAsync(GetOwnersRequest request)
	{
		var owners = await _ownerRepository.GetAllOwnersAsync(request);

		return new GetOwnersResponse
		{
			Result = owners,
			Offset = request.Offset,
			Total = owners.Count
		};
	}

	public async Task AddOwnerAsync(AddOwnerRequest request)
	{
		var owner = new Owner
		{
			Name = request.Name,
			Cars = new List<Car>()
		};

		await _ownerRepository.AddOwnerAsync(owner);
	}

	public async Task DeleteOwnerAsync(int id)
	{
		var owner = await _ownerRepository.GetOwnerByIdAsync(id);
		if (owner == null)
		{
			throw new ArgumentException($"Owner with ID {id} was not found.");
		}

		await _ownerRepository.DeleteOwnerAsync(id);
	}

	public async Task UpdateOwnerAsync(int id, UpdateOwnerRequest request)
	{
		var existingOwner = await _ownerRepository.GetOwnerByIdAsync(id);
		if (existingOwner == null)
		{
			throw new ArgumentException($"Owner with ID {id} was not found.");
		}

		existingOwner.Name = request.Name;
		await _ownerRepository.UpdateOwnerAsync(existingOwner);
	}
}
