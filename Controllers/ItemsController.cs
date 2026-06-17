using FridgeApp.Entities;
using FridgeApp.Enums;
using FridgeApp.Interfaces;
using FridgeApp.Models;
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

		[HttpGet("expiring")]
		public async Task<IActionResult> GetExpiringItems([FromQuery] int fridgeId, [FromQuery] string filter)
		{
			try
			{
				var items = await _itemService.GetExpiringItemsAsync(fridgeId, filter);
				return Ok(items);
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new { message = exception.Message });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Add(CreateItemRequest request)
		{
			try
			{
				var createdItem = await _itemService.CreateItem(request);
				return Ok(createdItem);
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new { message = exception.Message });
			}
		}

		[HttpPatch("{id}/quantity")]
		public async Task<IActionResult> UpdateQuantity(int id, [FromQuery] int delta)
		{
			try
			{
				var item = await _itemService.UpdateCountableQuantityAsync(id, delta);

				if (item is null)
				{
					return NotFound(new { message = "Item bulunamadi." });
				}

				return Ok(MapQuickUpdateResponse(item));
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new { message = exception.Message });
			}
		}

		[HttpPatch("{id}/approximate-status")]
		public async Task<IActionResult> UpdateApproximateStatus(int id, [FromQuery] ApproximateItemStatus status)
		{
			try
			{
				var item = await _itemService.UpdateApproximateStatusAsync(id, status);

				if (item is null)
				{
					return NotFound(new { message = "Item bulunamadi." });
				}

				return Ok(MapQuickUpdateResponse(item));
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new { message = exception.Message });
			}
		}

		[HttpPatch("{id}/finish")]
		public async Task<IActionResult> Finish(int id)
		{
			var item = await _itemService.MarkAsFinishedAsync(id);

			if (item is null)
			{
				return NotFound(new { message = "Item bulunamadi." });
			}

			return Ok(MapQuickUpdateResponse(item));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var deleted = await _itemService.DeleteItemAsync(id);

			if (!deleted)
			{
				return NotFound(new { message = "Item bulunamadi." });
			}

			return NoContent();
		}

		private static ItemQuickUpdateResponse MapQuickUpdateResponse(Item item)
		{
			return new ItemQuickUpdateResponse
			{
				Id = item.Id,
				Name = item.Name,
				Quantity = item.Quantity,
				Unit = item.Unit,
				TrackingType = item.TrackingType,
				ApproximateStatus = item.ApproximateStatus,
				IsFinished = item.IsDeleted || item.Quantity <= 0 || item.ApproximateStatus == ApproximateItemStatus.Finished
			};
		}
	}
}
