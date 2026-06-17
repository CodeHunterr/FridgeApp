namespace FridgeApp.Models
{
	public class RecipeSuggestionResponse
	{
		public string RecipeName { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? Category { get; set; }
		public int? EstimatedPrepTimeMinutes { get; set; }
		public List<string> MatchedItems { get; set; } = [];
		public List<string> MissingItems { get; set; } = [];
		public List<string> Ingredients { get; set; } = [];
		public List<string> PreparationSteps { get; set; } = [];
		public int Score { get; set; }
		public bool CanMakeNow { get; set; }
	}
}
