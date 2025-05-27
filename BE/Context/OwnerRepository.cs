using Microsoft.EntityFrameworkCore;
using System;

namespace BE.Context;

public class OwnerRepository : IOwnerRepository
{
	private readonly DataContext _context;

	public OwnerRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<List<Owner>> GetAllOwnersAsync(GetOwnersRequest request)
	{
		var query = _context.Owners.Include(o => o.Cars).AsQueryable();

		if (!string.IsNullOrWhiteSpace(request.NameFilter))
			query = query.Where(o => o.Name.Contains(request.NameFilter));

		var filtered = await query
			.Skip(request.Offset)
			.Take(request.Limit)
			.ToListAsync();
		return filtered;
	}

	public async Task<Owner?> GetOwnerByIdAsync(int id)
	{
		return await _context.Owners.Include(o => o.Cars).FirstOrDefaultAsync(o => o.Id == id);
	}

	public async Task AddOwnerAsync(Owner owner)
	{
		_context.Owners.Add(owner);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateOwnerAsync(Owner owner)
	{
		_context.Owners.Update(owner);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteOwnerAsync(int id)
	{
		var owner = await _context.Owners.FindAsync(id);
		if (owner != null)
		{
			_context.Owners.Remove(owner);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<List<Owner>> GetOwnersWithCarsAsync(string? ownerNameFilter, string? brandFilter)
	{
		var query = _context.Owners
			.Include(o => o.Cars)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(ownerNameFilter))
			query = query.Where(o => o.Name.Contains(ownerNameFilter));

		if (!string.IsNullOrWhiteSpace(brandFilter))
			query = query.Where(o => o.Cars.Any(c => c.Brand.Contains(brandFilter)));

		return await query
			.AsNoTracking()
			.ToListAsync();
	}
}
