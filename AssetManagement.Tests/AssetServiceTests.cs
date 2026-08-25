using AssetManagement.Business.DTOs;
using AssetManagement.Business.Services;
using AssetManagement.Core.Entities;
using AssetManagement.DataAccess.Repositories;
using Moq;

namespace AssetManagement.Tests
{
	public class AssetServiceTests
	{
		private readonly Mock<IAssetRepository> _repositoryMock;
		private readonly AssetService _service;

		public AssetServiceTests()
		{
			_repositoryMock = new Mock<IAssetRepository>();
			_service = new AssetService(_repositoryMock.Object);
		}

		[Fact]
		public async Task CreateAssetAsync_ValidDto_ReturnsResponseDto()
		{
			// Arrange
			var dto = new CreateAssetDto("MacBook Pro", "SN-998811", "Laptop");
			_repositoryMock.Setup(r => r.AddAsync(It.IsAny<Asset>()))
										 .ReturnsAsync((Asset a) => a);

			// Act
			var result = await _service.CreateAssetAsync(dto);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("MacBook Pro", result.Name);
			Assert.Equal("SN-998811", result.SerialNumber);
		}

		[Fact]
		public async Task CreateAssetAsync_EmptyName_ThrowsArgumentException()
		{
			// Arrange
			var dto = new CreateAssetDto("", "SN-998811", "Laptop");

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAssetAsync(dto));
		}
	}
}
