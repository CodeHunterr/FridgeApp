using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IProductCatalogService
	{
		Task<List<ProductCatalogItemResponse>> GetQuickAddProductsAsync();
		Task<List<ProductCatalogItemResponse>> SearchProductsAsync(string? query, string? category);
		Task<List<string>> GetCategoriesAsync();
	}
}
