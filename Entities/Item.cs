using FridgeApp.Enums;

namespace FridgeApp.Entities
{
	public class Item
	{
		public int Id { get; set; }
		public int FridgeId { get; set; }
		public int? ProductDefinitionId { get; set; }
		public string Name { get; set; } = string.Empty;
		public double Quantity { get; set; }
		public string Unit { get; set; } = string.Empty;
		public DateTime? ExpirationDate { get; set; }
		public TrackingType TrackingType { get; set; }
		public ApproximateItemStatus ApproximateStatus { get; set; }
		public ItemType ItemType { get; set; }
		public bool IsOpened { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedAt { get; set; }
		public DateTime AddedDate { get; set; }
	}
}
