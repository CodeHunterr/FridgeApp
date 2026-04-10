using FridgeApp.Entities;
using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IFridgeService
	{
		Task<Fridge> CreateFridge(CreateFridgeRequest fridgeRequest);
		Task<List<Fridge>> GetFridges();
	}
}
