using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IRecipeSuggestionService
	{
		Task<List<RecipeSuggestionResponse>> GetSuggestionsAsync(int fridgeId);
	}
}
