using AssetManagement.Core.Entities;

namespace AssetManagement.Business.DTOs
{
	public record CreateAssetDto(string Name, string SerialNumber, string Category);
	public record AssetResponseDto(Guid Id, string Name, string SerialNumber, string Category, AssetStatus Status, DateTime CreatedAt);
}
