using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BE.Context;

public class DataContext : DbContext
{
	public DbSet<Car> Cars { get; set; }
	public DbSet<Owner> Owners { get; set; }
	public DbSet<StoredFile> StoredFiles { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<MonitoredUser> MonitoredUsers { get; set; }
	public DbSet<UserActionLog> UserActionLogs { get; set; }

	private readonly IConfiguration _configuration;
	public DataContext(DbContextOptions options, IConfiguration configuration) : base(options)
	{
		_configuration = configuration;
	}

	public DataContext()
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer("Host=dpg-d0r0q27diees73bnrjig-a;Database=carmanagerdb;Username=carmanagerdb_user;Password=mVUxtnIUIlh6KwMc4LqLxkCUCcDoTRX0");
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Car>()
			.HasIndex(c => c.Brand);

		modelBuilder.Entity<Owner>()
			.HasIndex(o => o.Name);
	}
}
