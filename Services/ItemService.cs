using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
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

		public async Task<Item> CreateItem(Item item)
		{
			item.AddedDate = DateTime.Now;

			_context.Items.Add(item);
			await _context.SaveChangesAsync();

			return item;
		}

		public async Task<List<Item>> GetItems()
		{
			return await _context.Items.ToListAsync();
		}
	}
}
