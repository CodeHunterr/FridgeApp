using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FridgeApp.Services
{
	public class FridgeQuickAddService : IFridgeQuickAddService
	{
		private readonly AppDbContext _context;

		public FridgeQuickAddService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<ProductCatalogItemResponse>> GetQuickAddItemsAsync(int fridgeId)
		{
			var catalogProducts = await _context.ProductDefinitions
				.Where(product => product.IsActive && product.IsQuickAdd)
				.OrderBy(product => product.Name)
				.ToListAsync();

			var customFavorites = await _context.FridgeQuickAddItems
				.Where(item => item.FridgeId == fridgeId && item.IsActive)
				.OrderBy(item => item.SortOrder)
				.ThenBy(item => item.Name)
				.ToListAsync();

			var quickAddItems = new List<ProductCatalogItemResponse>();
			quickAddItems.AddRange(customFavorites.Select(MapCustomFavorite));
			quickAddItems.AddRange(catalogProducts.Select(MapCatalogProduct));

			return quickAddItems;
		}

		public async Task<ProductCatalogItemResponse> AddQuickAddItemAsync(
			int fridgeId,
			CreateFridgeQuickAddItemRequest request)
		{
			await EnsureFridgeExistsAsync(fridgeId);

			var fridgeExists = await _context.Fridges.AnyAsync(fridge => fridge.Id == fridgeId);
			if (!fridgeExists)
			{
				throw new InvalidOperationException("Dolap bulunamadı.");
			}

			ProductDefinition? productDefinition = null;
			if (request.ProductDefinitionId.HasValue)
			{
				productDefinition = await _context.ProductDefinitions
					.FirstOrDefaultAsync(product =>
						product.Id == request.ProductDefinitionId.Value &&
						product.IsActive);

				if (productDefinition is null)
				{
					throw new InvalidOperationException("Seçilen katalog ürünü bulunamadı.");
				}
			}

			var name = NormalizeSpacing(request.Name) ?? productDefinition?.Name;
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new InvalidOperationException("Favori ürün adı boş olamaz.");
			}

			var defaultUnit = NormalizeSpacing(request.DefaultUnit) ?? productDefinition?.DefaultUnit;
			if (string.IsNullOrWhiteSpace(defaultUnit))
			{
				throw new InvalidOperationException("Favori ürün birimi boş olamaz.");
			}

			var trackingType = request.TrackingType ?? productDefinition?.TrackingType;
			if (!trackingType.HasValue)
			{
				throw new InvalidOperationException("Takip tipi seçilmelidir.");
			}

			var normalizedName = NormalizeName(name);
			var catalogQuickAddNames = await _context.ProductDefinitions
				.Where(product => product.IsActive && product.IsQuickAdd)
				.Select(product => product.Name)
				.ToListAsync();
			var favoriteNames = await _context.FridgeQuickAddItems
				.Where(item => item.FridgeId == fridgeId && item.IsActive)
				.Select(item => item.Name)
				.ToListAsync();

			var duplicateCatalogExists = catalogQuickAddNames
				.Any(existingName => NormalizeName(existingName) == normalizedName);
			var duplicateFavoriteExists = favoriteNames
				.Any(existingName => NormalizeName(existingName) == normalizedName);

			if (duplicateCatalogExists || duplicateFavoriteExists)
			{
				throw new InvalidOperationException("Bu ürün zaten Hızlı Ekle listesinde.");
			}

			var quickAmounts = request.QuickAmounts?.Count > 0
				? request.QuickAmounts
				: productDefinition is not null
					? ParseQuickAmounts(productDefinition.QuickAmounts)
					: BuildDefaultQuickAmounts(defaultUnit);
			var nextSortOrder =
				await _context.FridgeQuickAddItems
					.Where(item => item.FridgeId == fridgeId)
					.Select(item => (int?)item.SortOrder)
					.MaxAsync() ?? 0;

			var quickAddItem = new FridgeQuickAddItem
			{
				FridgeId = fridgeId,
				ProductDefinitionId = request.ProductDefinitionId,
				Name = name,
				DefaultUnit = defaultUnit,
				TrackingType = trackingType.Value,
				QuickAmounts = SerializeQuickAmounts(quickAmounts),
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				SortOrder = nextSortOrder + 1
			};

			_context.FridgeQuickAddItems.Add(quickAddItem);
			await _context.SaveChangesAsync();

			return MapCustomFavorite(quickAddItem);
		}

		public async Task<bool> RemoveQuickAddItemAsync(int fridgeId, int quickAddItemId)
		{
			var item = await _context.FridgeQuickAddItems
				.FirstOrDefaultAsync(currentItem =>
					currentItem.Id == quickAddItemId &&
					currentItem.FridgeId == fridgeId &&
					currentItem.IsActive);

			if (item is null)
			{
				return false;
			}

			item.IsActive = false;
			await _context.SaveChangesAsync();
			return true;
		}

		private async Task EnsureFridgeExistsAsync(int fridgeId)
		{
			if (fridgeId != 1)
			{
				return;
			}

			var fridgeExists = await _context.Fridges.AnyAsync(fridge => fridge.Id == fridgeId);
			if (fridgeExists)
			{
				return;
			}

			_context.Fridges.Add(new Fridge
			{
				Id = 1,
				UserId = 1,
				Name = "Varsayılan Dolap"
			});

			await _context.SaveChangesAsync();
		}

		private static ProductCatalogItemResponse MapCatalogProduct(ProductDefinition product)
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

		private static ProductCatalogItemResponse MapCustomFavorite(FridgeQuickAddItem item)
		{
			return new ProductCatalogItemResponse
			{
				Id = item.Id,
				ProductDefinitionId = item.ProductDefinitionId,
				QuickAddItemId = item.Id,
				Name = item.Name,
				Category = "Favori",
				SubCategory = "Özel",
				DefaultUnit = item.DefaultUnit,
				QuickAmounts = ParseQuickAmounts(item.QuickAmounts),
				TrackingType = item.TrackingType,
				IsQuickAdd = true,
				IsCustom = true
			};
		}

		private static List<double> BuildDefaultQuickAmounts(string unit)
		{
			return NormalizeUnit(unit) switch
			{
				"kg" or "kilo" or "kilogram" => [0.5, 1, 2],
				"gr" or "gram" => [100, 250, 500],
				"litre" or "lt" => [1, 2],
				"ml" or "mililitre" => [250, 500, 1000],
				_ => [1, 2, 3]
			};
		}

		private static string SerializeQuickAmounts(List<double> quickAmounts)
		{
			return JsonSerializer.Serialize(quickAmounts
				.Where(amount => amount > 0)
				.Distinct()
				.OrderBy(amount => amount)
				.ToList());
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

		private static string? NormalizeSpacing(string? value)
		{
			var trimmed = value?.Trim() ?? string.Empty;
			if (string.IsNullOrWhiteSpace(trimmed))
			{
				return null;
			}

			return string.Join(' ', trimmed
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
		}

		private static string NormalizeName(string value)
		{
			return (NormalizeSpacing(value) ?? string.Empty)
				.ToLowerInvariant()
				.Replace('ç', 'c')
				.Replace('ğ', 'g')
				.Replace('ı', 'i')
				.Replace('ö', 'o')
				.Replace('ş', 's')
				.Replace('ü', 'u');
		}

		private static string NormalizeUnit(string value)
		{
			return NormalizeName(value);
		}
	}
}
