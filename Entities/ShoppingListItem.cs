namespace FridgeApp.Entities
{
	public class ShoppingListItem
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Source { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
