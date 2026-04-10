using FridgeApp.Entities;

namespace FridgeApp.Interfaces
{
	public interface IItemService
	{
		Task<Item> CreateItem(Item item);
		Task<List<Item>> GetItems();
	}
}
