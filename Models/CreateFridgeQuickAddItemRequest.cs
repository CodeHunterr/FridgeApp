using FridgeApp.Enums;

namespace FridgeApp.Models
{
	public class CreateFridgeQuickAddItemRequest
	{
		public int? ProductDefinitionId { get; set; }
		public string? Name { get; set; }
		public string? DefaultUnit { get; set; }
		public TrackingType? TrackingType { get; set; }
		public List<double>? QuickAmounts { get; set; }
	}
}
