using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Enums;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class ItemService : IItemService
	{
		private readonly AppDbContext _context;

		public ItemService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Item> CreateItem(CreateItemRequest request)
		{
			ProductDefinition? productDefinition = null;

			if (request.ProductDefinitionId.HasValue)
			{
				productDefinition = await _context.ProductDefinitions
					.FirstOrDefaultAsync(product => product.Id == request.ProductDefinitionId.Value && product.IsActive);

				if (productDefinition is null)
				{
					throw new InvalidOperationException("Secilen urun katalogda bulunamadi.");
				}
			}

			var itemName = !string.IsNullOrWhiteSpace(request.Name)
				? request.Name.Trim()
				: productDefinition?.Name;

			if (string.IsNullOrWhiteSpace(itemName))
			{
				throw new InvalidOperationException("Urun adi belirlenemedi.");
			}

			var unit = !string.IsNullOrWhiteSpace(request.Unit)
				? request.Unit.Trim()
				: productDefinition?.DefaultUnit ?? string.Empty;

			var trackingType = productDefinition?.TrackingType
				?? request.TrackingType
				?? TrackingType.Countable;

			var item = new Item
			{
				FridgeId = request.FridgeId,
				ProductDefinitionId = request.ProductDefinitionId,
				Name = itemName,
				Quantity = request.Quantity,
				Unit = unit,
				ExpirationDate = NormalizeExpirationDate(request.ExpirationDate),
				TrackingType = trackingType,
				ApproximateStatus = ApproximateItemStatus.Unknown,
				ItemType = request.ItemType,
				IsOpened = request.IsOpened,
				IsDeleted = false,
				AddedDate = DateTime.UtcNow
			};

			_context.Items.Add(item);
			await AddActivityLogAsync(
				request.FridgeId,
				item.Name,
				item.ProductDefinitionId,
				ItemActivityActionType.Added,
				item.Quantity,
				item.Unit);

			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<List<Item>> GetItems()
		{
			return await _context.Items
				.Where(item => !item.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Item>> GetExpiringItemsAsync(int fridgeId, string filter)
		{
			var today = DateTime.UtcNow.Date;
			var itemsQuery = _context.Items
				.Where(item => item.FridgeId == fridgeId && !item.IsDeleted && item.ExpirationDate.HasValue)
				.AsQueryable();

			itemsQuery = filter.ToLowerInvariant() switch
			{
				"expired" => itemsQuery.Where(item => item.ExpirationDate!.Value.Date < today),
				"next3days" => itemsQuery.Where(item =>
					item.ExpirationDate!.Value.Date >= today &&
					item.ExpirationDate.Value.Date <= today.AddDays(3)),
				"next7days" => itemsQuery.Where(item =>
					item.ExpirationDate!.Value.Date >= today &&
					item.ExpirationDate.Value.Date <= today.AddDays(7)),
				_ => throw new InvalidOperationException("Gecersiz filtre. expired, next3Days veya next7Days kullanin.")
			};

			return await itemsQuery
				.OrderBy(item => item.ExpirationDate)
				.ToListAsync();
		}

		public async Task<Item?> UpdateCountableQuantityAsync(int id, int delta)
		{
			var item = await _context.Items
				.FirstOrDefaultAsync(currentItem => currentItem.Id == id && !currentItem.IsDeleted);

			if (item is null)
			{
				return null;
			}

			if (item.TrackingType != TrackingType.Countable)
			{
				throw new InvalidOperationException("Bu urun countable takip tipinde degil.");
			}

			if (delta == 0)
			{
				throw new InvalidOperationException("Delta 0 olamaz.");
			}

			item.Quantity = Math.Max(0, item.Quantity + delta);

			await AddActivityLogAsync(
				item.FridgeId,
				item.Name,
				item.ProductDefinitionId,
				delta > 0 ? ItemActivityActionType.Increased : ItemActivityActionType.Decreased,
				Math.Abs(delta),
				item.Unit);

			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<Item?> UpdateApproximateStatusAsync(int id, ApproximateItemStatus status)
		{
			var item = await _context.Items
				.FirstOrDefaultAsync(currentItem => currentItem.Id == id && !currentItem.IsDeleted);

			if (item is null)
			{
				return null;
			}

			var actionType = status switch
			{
				ApproximateItemStatus.Low => ItemActivityActionType.MarkedLow,
				ApproximateItemStatus.Half => ItemActivityActionType.MarkedHalf,
				ApproximateItemStatus.AlmostFinished => ItemActivityActionType.MarkedAlmostFinished,
				ApproximateItemStatus.Finished => ItemActivityActionType.Finished,
				_ => throw new InvalidOperationException("Gecersiz approximate durum gonderildi.")
			};

			item.ApproximateStatus = status;

			if (status == ApproximateItemStatus.Finished)
			{
				item.Quantity = 0;
				item.IsDeleted = true;
				item.DeletedAt = DateTime.UtcNow;
			}

			await AddActivityLogAsync(
				item.FridgeId,
				item.Name,
				item.ProductDefinitionId,
				actionType,
				item.Quantity,
				item.Unit);

			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<Item?> MarkAsFinishedAsync(int id)
		{
			var item = await _context.Items
				.FirstOrDefaultAsync(currentItem => currentItem.Id == id && !currentItem.IsDeleted);

			if (item is null)
			{
				return null;
			}

			item.Quantity = 0;
			item.IsDeleted = true;
			item.DeletedAt = DateTime.UtcNow;

			if (item.TrackingType == TrackingType.Approximate)
			{
				item.ApproximateStatus = ApproximateItemStatus.Finished;
			}

			await AddActivityLogAsync(
				item.FridgeId,
				item.Name,
				item.ProductDefinitionId,
				ItemActivityActionType.Finished,
				item.Quantity,
				item.Unit);

			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<bool> DeleteItemAsync(int id)
		{
			var item = await _context.Items
				.FirstOrDefaultAsync(currentItem => currentItem.Id == id && !currentItem.IsDeleted);

			if (item is null)
			{
				return false;
			}

			item.IsDeleted = true;
			item.DeletedAt = DateTime.UtcNow;

			await AddActivityLogAsync(
				item.FridgeId,
				item.Name,
				item.ProductDefinitionId,
				ItemActivityActionType.Deleted,
				item.Quantity,
				item.Unit);

			await _context.SaveChangesAsync();

			return true;
		}

		private async Task AddActivityLogAsync(
			int fridgeId,
			string itemName,
			int? productDefinitionId,
			ItemActivityActionType actionType,
			double? quantity,
			string? unit)
		{
			var userId = await _context.Fridges
				.Where(fridge => fridge.Id == fridgeId)
				.Select(fridge => fridge.UserId)
				.FirstOrDefaultAsync();

			var log = new ItemActivityLog
			{
				UserId = userId,
				FridgeId = fridgeId,
				ItemName = itemName,
				ProductDefinitionId = productDefinitionId,
				ActionType = actionType,
				Quantity = quantity,
				Unit = unit,
				CreatedAt = DateTime.UtcNow
			};

			_context.ItemActivityLogs.Add(log);
		}

		private static DateTime? NormalizeExpirationDate(DateTime? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			try
			{
				return DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc);
			}
			catch (ArgumentOutOfRangeException exception)
			{
				throw new InvalidOperationException("Gecersiz son kullanma tarihi.", exception);
			}
		}
	}
}
