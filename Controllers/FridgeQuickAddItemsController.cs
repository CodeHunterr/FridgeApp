using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/fridges/{fridgeId:int}/quick-add-items")]
	public class FridgeQuickAddItemsController : ControllerBase
	{
		private readonly IFridgeQuickAddService _fridgeQuickAddService;

		public FridgeQuickAddItemsController(IFridgeQuickAddService fridgeQuickAddService)
		{
			_fridgeQuickAddService = fridgeQuickAddService;
		}

		[HttpGet]
		public async Task<IActionResult> GetQuickAddItems(int fridgeId)
		{
			var items = await _fridgeQuickAddService.GetQuickAddItemsAsync(fridgeId);
			return Ok(items);
		}

		[HttpPost]
		public async Task<IActionResult> AddQuickAddItem(
			int fridgeId,
			[FromBody] CreateFridgeQuickAddItemRequest request)
		{
			try
			{
				var item = await _fridgeQuickAddService.AddQuickAddItemAsync(fridgeId, request);
				return Ok(item);
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new { message = exception.Message });
			}
		}

		[HttpDelete("{quickAddItemId:int}")]
		public async Task<IActionResult> RemoveQuickAddItem(int fridgeId, int quickAddItemId)
		{
			var removed = await _fridgeQuickAddService.RemoveQuickAddItemAsync(fridgeId, quickAddItemId);

			if (!removed)
			{
				return NotFound(new { message = "Favori hızlı ekle ürünü bulunamadı." });
			}

			return NoContent();
		}
	}
}
