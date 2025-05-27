using BE.Models;

namespace BE.Services;

public interface IMetricsService
{
	Task<Metrics> CalculateMetric();
	Task<List<OwnerCarStat>> GetCarCountPerOwnerAsync(string? ownerNameFilter = null, string? brandFilter = null);
	Task<List<OwnerCarStat>> GetCarCountPerOwnerUnoptimizedAsync(string? ownerNameFilter = null, string? brandFilter = null);
}
