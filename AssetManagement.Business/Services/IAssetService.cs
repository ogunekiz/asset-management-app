using AssetManagement.Business.DTOs;
using AssetManagement.Core.Entities;
using AssetManagement.DataAccess.Repositories;

namespace AssetManagement.Business.Services
{
	public interface IAssetService
	{
		Task<List<AssetResponseDto>> GetAllAssetsAsync();
		Task<AssetResponseDto> CreateAssetAsync(CreateAssetDto dto);
	}

	public class AssetService : IAssetService
	{
		private readonly IAssetRepository _repository;

		public AssetService(IAssetRepository repository)
		{
			_repository = repository;
		}

		public async Task<List<AssetResponseDto>> GetAllAssetsAsync()
		{
			var assets = await _repository.GetAllAsync();
			return assets.Select(a => new AssetResponseDto(a.Id, a.Name, a.SerialNumber, a.Category, a.Status, a.CreatedAt)).ToList();
		}

		public async Task<AssetResponseDto> CreateAssetAsync(CreateAssetDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.SerialNumber))
				throw new ArgumentException("Asset name and serial number cannot be empty.");

			var asset = new Asset
			{
				Name = dto.Name,
				SerialNumber = dto.SerialNumber,
				Category = dto.Category,
				Status = AssetStatus.InStock
			};

			var created = await _repository.AddAsync(asset);
			return new AssetResponseDto(created.Id, created.Name, created.SerialNumber, created.Category, created.Status, created.CreatedAt);
		}
	}
}
