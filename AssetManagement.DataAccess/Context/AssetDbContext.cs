using AssetManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.DataAccess.Context
{
	public class AssetDbContext : DbContext
	{
		public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) { }

		public DbSet<Asset> Assets => Set<Asset>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Asset>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(50);
				entity.HasIndex(e => e.SerialNumber).IsUnique();
			});
		}
	}
}
