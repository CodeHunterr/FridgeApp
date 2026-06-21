using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class DemoBootstrapService : IDemoBootstrapService
	{
		private const string DefaultDisplayName = "Demo Kullanıcı";
		private const string DefaultFridgeName = "Benim Dolabım";
		private readonly AppDbContext _context;

		public DemoBootstrapService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<DemoBootstrapResponse> BootstrapAsync(DemoBootstrapRequest request)
		{
			var installId = request.InstallId?.Trim();
			if (string.IsNullOrWhiteSpace(installId))
			{
				throw new ArgumentException("Kurulum kimliği gerekli.");
			}

			var displayName = Normalize(request.DisplayName, DefaultDisplayName, 50);
			var fridgeName = Normalize(request.FridgeName, DefaultFridgeName, 80);
			var user = await _context.Users
				.FirstOrDefaultAsync(current => current.InstallId == installId);

			if (user == null)
			{
				user = new User
				{
					InstallId = installId,
					DisplayName = displayName,
					Email = $"demo-{installId}@local.invalid",
					PasswordHash = string.Empty,
					CreatedDate = DateTime.UtcNow
				};

				_context.Users.Add(user);

				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateException)
				{
					_context.ChangeTracker.Clear();
					user = await _context.Users
						.FirstOrDefaultAsync(current => current.InstallId == installId);
					if (user == null)
					{
						throw;
					}
				}
			}

			user.DisplayName = displayName;
			var fridge = await _context.Fridges
				.Where(current => current.UserId == user.Id)
				.OrderBy(current => current.Id)
				.FirstOrDefaultAsync();

			if (fridge == null)
			{
				fridge = new Fridge
				{
					UserId = user.Id,
					Name = fridgeName
				};
				_context.Fridges.Add(fridge);
			}
			else
			{
				fridge.Name = fridgeName;
			}

			await _context.SaveChangesAsync();

			return new DemoBootstrapResponse
			{
				UserId = user.Id,
				FridgeId = fridge.Id,
				DisplayName = user.DisplayName,
				FridgeName = fridge.Name ?? DefaultFridgeName
			};
		}

		private static string Normalize(string? value, string fallback, int maxLength)
		{
			var trimmed = value?.Trim() ?? string.Empty;
			if (trimmed.Length == 0)
			{
				return fallback;
			}

			return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
		}
	}
}
