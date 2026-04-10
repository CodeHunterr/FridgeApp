namespace FridgeApp.Models
{
	public class CreateUserRequest
	{
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
	}
}
