using FridgeApp.Enums;

namespace FridgeApp.Entities
{
	public class FridgeQuickAddItem
	{
		public int Id { get; set; }
		public int FridgeId { get; set; }
		public int? ProductDefinitionId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string DefaultUnit { get; set; } = string.Empty;
		public TrackingType TrackingType { get; set; }
		public string QuickAmounts { get; set; } = "[]";
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; }
		public int SortOrder { get; set; }
	}
}
