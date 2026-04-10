using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FridgesController : ControllerBase
	{
		private readonly IFridgeService _fridgeService;

		public FridgesController(IFridgeService fridgeService)
		{
			_fridgeService = fridgeService;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateFridgeRequest fridgeRequest)
		{
			var fridge = await _fridgeService.CreateFridge(fridgeRequest);

			return Ok(fridge);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var fridges = await _fridgeService.GetFridges();

			return Ok(fridges);
		}
	}
}
