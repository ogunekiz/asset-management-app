using AssetManagement.Core.Entities;
using AssetManagement.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.DataAccess.Repositories
{
	public interface IAssetRepository
	{
		Task<List<Asset>> GetAllAsync();
		Task<Asset?> GetByIdAsync(Guid id);
		Task<Asset> AddAsync(Asset asset);
	}

	public class AssetRepository : IAssetRepository
	{
		private readonly AssetDbContext _context;

		public AssetRepository(AssetDbContext context)
		{
			_context = context;
		}

		public async Task<List<Asset>> GetAllAsync() => await _context.Assets.ToListAsync();

		public async Task<Asset?> GetByIdAsync(Guid id) => await _context.Assets.FindAsync(id);

		public async Task<Asset> AddAsync(Asset asset)
		{
			await _context.Assets.AddAsync(asset);
			await _context.SaveChangesAsync();
			return asset;
		}
	}
}
