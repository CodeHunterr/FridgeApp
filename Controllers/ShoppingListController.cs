using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/shopping-list")]
	public class ShoppingListController : ControllerBase
	{
		private readonly IShoppingListService _shoppingListService;

		public ShoppingListController(IShoppingListService shoppingListService)
		{
			_shoppingListService = shoppingListService;
		}

		[HttpGet]
		public async Task<IActionResult> GetList([FromQuery] int userId)
		{
			var items = await _shoppingListService.GetListAsync(userId);
			return Ok(items);
		}

		[HttpPost]
		public async Task<IActionResult> Add(CreateShoppingListItemRequest request)
		{
			var item = await _shoppingListService.AddItemAsync(request);
			return Ok(item);
		}

		[HttpPatch("{id}/complete")]
		public async Task<IActionResult> Complete(int id)
		{
			var item = await _shoppingListService.MarkCompletedAsync(id);

			if (item is null)
			{
				return NotFound(new { message = "Shopping list item bulunamadi." });
			}

			return Ok(item);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var deleted = await _shoppingListService.DeleteAsync(id);

			if (!deleted)
			{
				return NotFound(new { message = "Shopping list item bulunamadi." });
			}

			return NoContent();
		}
	}
}
