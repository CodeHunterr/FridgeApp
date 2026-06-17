using FridgeApp.Enums;

namespace FridgeApp.Entities
{
	public class ProductDefinition
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string? SubCategory { get; set; }
		public string DefaultUnit { get; set; } = string.Empty;
		public string QuickAmounts { get; set; } = "[]";
		public TrackingType TrackingType { get; set; }
		public bool IsQuickAdd { get; set; }
		public bool IsActive { get; set; } = true;
	}
}
