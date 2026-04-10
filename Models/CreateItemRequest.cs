namespace FridgeApp.Models
{
	public class CreateItemRequest
	{
		public int FridgeId { get; set; }

		public string Name { get; set; } = string.Empty;

		public double Quantity { get; set; }

		public string? Unit { get; set; }

		public DateTime? ExpirationDate { get; set; }
	}
}
