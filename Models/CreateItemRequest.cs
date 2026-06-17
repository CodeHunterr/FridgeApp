using FridgeApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace FridgeApp.Models
{
	public class CreateItemRequest : IValidatableObject
	{
		public int FridgeId { get; set; }

		public int? ProductDefinitionId { get; set; }

		public string? Name { get; set; }

		public double Quantity { get; set; }

		public string? Unit { get; set; }

		public DateTime? ExpirationDate { get; set; }

		public TrackingType? TrackingType { get; set; }

		public ItemType ItemType { get; set; }

		public bool IsOpened { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!ProductDefinitionId.HasValue && string.IsNullOrWhiteSpace(Name))
			{
				yield return new ValidationResult(
					"ProductDefinitionId veya Name alanlarindan en az biri dolu olmalidir.",
					new[] { nameof(ProductDefinitionId), nameof(Name) });
			}
		}
	}
}
