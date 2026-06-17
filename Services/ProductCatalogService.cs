using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FridgeApp.Services
{
	public class ProductCatalogService : IProductCatalogService
	{
		private readonly AppDbContext _context;

		public ProductCatalogService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<ProductCatalogItemResponse>> GetQuickAddProductsAsync()
		{
			var products = await _context.ProductDefinitions
				.Where(product => product.IsActive && product.IsQuickAdd)
				.OrderBy(product => product.Name)
				.ToListAsync();

			return products.Select(MapProduct).ToList();
		}

		public async Task<List<ProductCatalogItemResponse>> SearchProductsAsync(string? query, string? category)
		{
			var productsQuery = _context.ProductDefinitions
				.Where(product => product.IsActive)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(category))
			{
				productsQuery = productsQuery.Where(product => product.Category == category);
			}

			if (!string.IsNullOrWhiteSpace(query))
			{
				productsQuery = productsQuery.Where(product => product.Name.Contains(query));
			}

			var products = await productsQuery
				.OrderBy(product => product.Name)
				.ToListAsync();

			return products.Select(MapProduct).ToList();
		}

		public async Task<List<string>> GetCategoriesAsync()
		{
			return await _context.ProductDefinitions
				.Where(product => product.IsActive)
				.Select(product => product.Category)
				.Where(category => !string.IsNullOrWhiteSpace(category))
				.Distinct()
				.OrderBy(category => category)
				.ToListAsync();
		}

		private static ProductCatalogItemResponse MapProduct(ProductDefinition product)
		{
			return new ProductCatalogItemResponse
			{
				Id = product.Id,
				ProductDefinitionId = product.Id,
				QuickAddItemId = null,
				Name = product.Name,
				Category = product.Category,
				SubCategory = product.SubCategory,
				DefaultUnit = product.DefaultUnit,
				QuickAmounts = ParseQuickAmounts(product.QuickAmounts),
				TrackingType = product.TrackingType,
				IsQuickAdd = product.IsQuickAdd,
				IsCustom = false
			};
		}

		private static List<double> ParseQuickAmounts(string quickAmounts)
		{
			if (string.IsNullOrWhiteSpace(quickAmounts))
			{
				return [];
			}

			try
			{
				return JsonSerializer.Deserialize<List<double>>(quickAmounts) ?? [];
			}
			catch (JsonException)
			{
				return [];
			}
		}
	}
}
