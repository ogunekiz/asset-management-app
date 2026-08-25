using AssetManagement.Business.DTOs;
using AssetManagement.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AssetsController : ControllerBase
	{
		private readonly IAssetService _assetService;

		public AssetsController(IAssetService assetService)
		{
			_assetService = assetService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _assetService.GetAllAssetsAsync();
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateAssetDto dto)
		{
			var result = await _assetService.CreateAssetAsync(dto);
			return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
		}
	}
}
