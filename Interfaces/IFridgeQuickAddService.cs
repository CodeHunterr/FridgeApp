using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IFridgeQuickAddService
	{
		Task<List<ProductCatalogItemResponse>> GetQuickAddItemsAsync(int fridgeId);
		Task<ProductCatalogItemResponse> AddQuickAddItemAsync(int fridgeId, CreateFridgeQuickAddItemRequest request);
		Task<bool> RemoveQuickAddItemAsync(int fridgeId, int quickAddItemId);
	}
}
