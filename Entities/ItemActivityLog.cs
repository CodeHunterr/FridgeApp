using FridgeApp.Enums;

namespace FridgeApp.Entities
{
	public class ItemActivityLog
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int FridgeId { get; set; }
		public string ItemName { get; set; } = string.Empty;
		public int? ProductDefinitionId { get; set; }
		public ItemActivityActionType ActionType { get; set; }
		public double? Quantity { get; set; }
		public string? Unit { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
