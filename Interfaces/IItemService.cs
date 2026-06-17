using FridgeApp.Enums;
using FridgeApp.Entities;
using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IItemService
	{
		Task<Item> CreateItem(CreateItemRequest request);
		Task<List<Item>> GetItems();
		Task<List<Item>> GetExpiringItemsAsync(int fridgeId, string filter);
		Task<Item?> UpdateCountableQuantityAsync(int id, int delta);
		Task<Item?> UpdateApproximateStatusAsync(int id, ApproximateItemStatus status);
		Task<Item?> MarkAsFinishedAsync(int id);
		Task<bool> DeleteItemAsync(int id);
	}
}
