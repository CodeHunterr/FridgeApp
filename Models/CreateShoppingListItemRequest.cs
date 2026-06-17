using System.ComponentModel.DataAnnotations;

namespace FridgeApp.Models
{
	public class CreateShoppingListItemRequest
	{
		public int UserId { get; set; }

		[Required]
		public string Name { get; set; } = string.Empty;

		public string? Source { get; set; }
	}
}
