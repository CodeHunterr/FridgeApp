using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class FridgeService : IFridgeService
	{
		private readonly AppDbContext _context;

		public FridgeService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Fridge> CreateFridge(CreateFridgeRequest fridgeRequest)
		{
			var fridge = new Fridge
			{
				UserId = fridgeRequest.UserId,
				Name = fridgeRequest.Name
			};

			_context.Fridges.Add(fridge);
			await _context.SaveChangesAsync();

			return fridge;
		}

		public async Task<List<Fridge>> GetFridges()
		{
			return await _context.Fridges.ToListAsync();
		}
	}
}
