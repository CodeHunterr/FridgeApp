using FridgeApp.Entities;
using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IShoppingListService
	{
		Task<List<ShoppingListItem>> GetListAsync(int userId);
		Task<ShoppingListItem> AddItemAsync(CreateShoppingListItemRequest request);
		Task<ShoppingListItem?> MarkCompletedAsync(int id);
		Task<bool> DeleteAsync(int id);
	}
}
