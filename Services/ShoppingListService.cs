using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class ShoppingListService : IShoppingListService
	{
		private readonly AppDbContext _context;

		public ShoppingListService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<ShoppingListItem>> GetListAsync(int userId)
		{
			return await _context.ShoppingListItems
				.Where(item => item.UserId == userId)
				.OrderBy(item => item.IsCompleted)
				.ThenByDescending(item => item.CreatedAt)
				.ToListAsync();
		}

		public async Task<ShoppingListItem> AddItemAsync(CreateShoppingListItemRequest request)
		{
			var item = new ShoppingListItem
			{
				UserId = request.UserId,
				Name = request.Name.Trim(),
				Source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source.Trim(),
				IsCompleted = false,
				CreatedAt = DateTime.UtcNow
			};

			_context.ShoppingListItems.Add(item);
			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<ShoppingListItem?> MarkCompletedAsync(int id)
		{
			var item = await _context.ShoppingListItems.FindAsync(id);

			if (item is null)
			{
				return null;
			}

			item.IsCompleted = true;
			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var item = await _context.ShoppingListItems.FindAsync(id);

			if (item is null)
			{
				return false;
			}

			_context.ShoppingListItems.Remove(item);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
