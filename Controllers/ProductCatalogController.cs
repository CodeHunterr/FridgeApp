using FridgeApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductCatalogController : ControllerBase
	{
		private readonly IProductCatalogService _productCatalogService;

		public ProductCatalogController(IProductCatalogService productCatalogService)
		{
			_productCatalogService = productCatalogService;
		}

		[HttpGet("quick-add")]
		public async Task<IActionResult> GetQuickAddProducts()
		{
			var products = await _productCatalogService.GetQuickAddProductsAsync();
			return Ok(products);
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchProducts([FromQuery] string? query, [FromQuery] string? category)
		{
			var products = await _productCatalogService.SearchProductsAsync(query, category);
			return Ok(products);
		}

		[HttpGet("categories")]
		public async Task<IActionResult> GetCategories()
		{
			var categories = await _productCatalogService.GetCategoriesAsync();
			return Ok(categories);
		}
	}
}
