using FridgeApp.Entities;
using FridgeApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ItemsController : ControllerBase
	{
		private readonly IItemService _itemService;

		public ItemsController(IItemService itemService)
		{
			_itemService = itemService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var items = await _itemService.GetItems();
			return Ok(items);
		}

		[HttpPost]
		public async Task<IActionResult> Add(Item item)
		{
			var createdItem = await _itemService.CreateItem(item);
			return Ok(createdItem);
		}
	}
}
