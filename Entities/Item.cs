namespace FridgeApp.Entities
{
	public class Item
	{
		public int Id { get; set; }
		public int FridgeId { get; set; }
		public string Name { get; set; }
		public double Quantity { get; set; }
		public string Unit { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public DateTime AddedDate { get; set; }
	}
}
