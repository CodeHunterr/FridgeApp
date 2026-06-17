using FridgeApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RecipesController : ControllerBase
	{
		private readonly IRecipeSuggestionService _recipeSuggestionService;

		public RecipesController(IRecipeSuggestionService recipeSuggestionService)
		{
			_recipeSuggestionService = recipeSuggestionService;
		}

		[HttpGet("suggestions")]
		public async Task<IActionResult> GetSuggestions([FromQuery] int fridgeId)
		{
			var suggestions = await _recipeSuggestionService.GetSuggestionsAsync(fridgeId);
			return Ok(suggestions);
		}
	}
}
