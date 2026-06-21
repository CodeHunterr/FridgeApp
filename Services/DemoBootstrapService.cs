using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FridgeApp.Services
{
	public class DemoBootstrapService : IDemoBootstrapService
	{
		private const string DefaultDisplayName = "Demo Kullanıcı";
		private const string DefaultFridgeName = "Benim Dolabım";
		private readonly AppDbContext _context;
		private readonly ILogger<DemoBootstrapService> _logger;

		public DemoBootstrapService(
			AppDbContext context,
			ILogger<DemoBootstrapService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<DemoBootstrapResponse> BootstrapAsync(DemoBootstrapRequest request)
		{
			var installId = NormalizeInstallId(request.InstallId);

			var displayName = Normalize(request.DisplayName, DefaultDisplayName, 50);
			var fridgeName = Normalize(request.FridgeName, DefaultFridgeName, 80);

			try
			{
				return await BootstrapWithinTransactionAsync(installId, displayName, fridgeName);
			}
			catch (DbUpdateException exception) when (IsInstallIdConflict(exception))
			{
				// Another request created this install id first. Reuse its committed records.
				_logger.LogInformation(
					"Demo bootstrap encountered a concurrent install id creation; loading the existing records.");
				_context.ChangeTracker.Clear();

				DemoBootstrapResponse? existingResponse;
				try
				{
					existingResponse = await FindExistingResponseAsync(installId);
				}
				catch (Exception lookupException) when (lookupException is not OperationCanceledException)
				{
					_logger.LogError(
						lookupException,
						"Demo bootstrap could not load records after a concurrent install id creation.");
					throw new DemoBootstrapPersistenceException();
				}

				if (existingResponse != null)
				{
					return existingResponse;
				}

				_logger.LogError(
					exception,
					"Demo bootstrap unique install id conflict did not yield an existing user.");
				throw new DemoBootstrapPersistenceException();
			}
			catch (DbUpdateException exception)
			{
				_logger.LogError(
					exception,
					"Demo bootstrap database save failed. Install id length: {InstallIdLength}.",
					installId.Length);
				throw new DemoBootstrapPersistenceException();
			}
			catch (DemoBootstrapPersistenceException)
			{
				throw;
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				_logger.LogError(
					exception,
					"Demo bootstrap failed unexpectedly. Install id length: {InstallIdLength}.",
					installId.Length);
				throw new DemoBootstrapPersistenceException();
			}
		}

		private async Task<DemoBootstrapResponse> BootstrapWithinTransactionAsync(
			string installId,
			string displayName,
			string fridgeName)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var existingResponse = await FindExistingResponseAsync(installId);
				if (existingResponse != null)
				{
					await transaction.CommitAsync();
					_logger.LogInformation(
						"Demo bootstrap reused user {UserId} and fridge {FridgeId}.",
						existingResponse.UserId,
						existingResponse.FridgeId);
					return existingResponse;
				}

				var user = new User
				{
					InstallId = installId,
					DisplayName = displayName,
					Email = $"demo-{installId}@local.invalid",
					PasswordHash = string.Empty,
					CreatedDate = DateTime.UtcNow
				};

				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				var fridge = new Fridge
				{
					UserId = user.Id,
					Name = fridgeName
				};

				_context.Fridges.Add(fridge);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				_logger.LogInformation(
					"Demo bootstrap created user {UserId} and fridge {FridgeId}.",
					user.Id,
					fridge.Id);

				return CreateResponse(user, fridge);
			}
			catch
			{
				await transaction.RollbackAsync();
				_context.ChangeTracker.Clear();
				throw;
			}
		}

		private async Task<DemoBootstrapResponse?> FindExistingResponseAsync(string installId)
		{
			var user = await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(current => current.InstallId == installId);

			if (user == null)
			{
				return null;
			}

			var fridge = await _context.Fridges
				.AsNoTracking()
				.Where(current => current.UserId == user.Id)
				.OrderBy(current => current.Id)
				.FirstOrDefaultAsync();

			if (fridge == null)
			{
				fridge = new Fridge
				{
					UserId = user.Id,
					Name = DefaultFridgeName
				};

				_context.Fridges.Add(fridge);
				await _context.SaveChangesAsync();
			}

			return CreateResponse(user, fridge);
		}

		private static DemoBootstrapResponse CreateResponse(User user, Fridge fridge)
		{
			return new DemoBootstrapResponse
			{
				UserId = user.Id,
				FridgeId = fridge.Id,
				DisplayName = string.IsNullOrWhiteSpace(user.DisplayName)
					? DefaultDisplayName
					: user.DisplayName,
				FridgeName = string.IsNullOrWhiteSpace(fridge.Name)
					? DefaultFridgeName
					: fridge.Name
			};
		}

		private static string NormalizeInstallId(string? value)
		{
			var installId = value?.Trim() ?? string.Empty;
			if (installId.Length == 0)
			{
				throw new ArgumentException("Kurulum kimliği gerekli.");
			}

			if (installId.Length > 120)
			{
				throw new ArgumentException("Kurulum kimliği en fazla 120 karakter olabilir.");
			}

			return installId;
		}

		private static bool IsInstallIdConflict(DbUpdateException exception)
		{
			return exception.InnerException is PostgresException postgresException &&
				postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
				postgresException.ConstraintName == "IX_Users_InstallId";
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

	public sealed class DemoBootstrapPersistenceException : Exception
	{
		public DemoBootstrapPersistenceException()
			: base("Demo bootstrap verileri kaydedilemedi.")
		{
		}
	}
}
