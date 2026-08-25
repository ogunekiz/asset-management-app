namespace AssetManagement.Core.Entities
{
	public enum AssetStatus
	{
		InStock = 1,
		Assigned = 2,
		InRepair = 3,
		Scrapped = 4
	}

	public class Asset
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; } = string.Empty;
		public string SerialNumber { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public AssetStatus Status { get; set; } = AssetStatus.InStock;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
