using FridgeApp.Enums;

namespace FridgeApp.Models
{
	public class ProductCatalogItemResponse
	{
		public int Id { get; set; }
		public int? ProductDefinitionId { get; set; }
		public int? QuickAddItemId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string? SubCategory { get; set; }
		public string DefaultUnit { get; set; } = string.Empty;
		public List<double> QuickAmounts { get; set; } = [];
		public TrackingType TrackingType { get; set; }
		public bool IsQuickAdd { get; set; }
		public bool IsCustom { get; set; }
	}
}
