using FridgeApp.Enums;

namespace FridgeApp.Models
{
	public class ItemQuickUpdateResponse
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double Quantity { get; set; }
		public string Unit { get; set; } = string.Empty;
		public TrackingType TrackingType { get; set; }
		public ApproximateItemStatus ApproximateStatus { get; set; }
		public bool IsFinished { get; set; }
	}
}
