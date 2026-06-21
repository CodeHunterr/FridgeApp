using FridgeApp.Interfaces;
using FridgeApp.Models;
using FridgeApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/demo/bootstrap")]
	public class DemoBootstrapController : ControllerBase
	{
		private readonly IDemoBootstrapService _demoBootstrapService;
		private readonly ILogger<DemoBootstrapController> _logger;

		public DemoBootstrapController(
			IDemoBootstrapService demoBootstrapService,
			ILogger<DemoBootstrapController> logger)
		{
			_demoBootstrapService = demoBootstrapService;
			_logger = logger;
		}

		[HttpPost]
		public async Task<ActionResult<DemoBootstrapResponse>> Bootstrap(
			[FromBody] DemoBootstrapRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.InstallId))
			{
				return BadRequest(new { message = "Kurulum kimliği gerekli." });
			}

			try
			{
				var response = await _demoBootstrapService.BootstrapAsync(request);
				return Ok(response);
			}
			catch (ArgumentException exception)
			{
				_logger.LogInformation(exception, "Demo bootstrap request was rejected.");
				return BadRequest(new { message = exception.Message });
			}
			catch (DemoBootstrapPersistenceException)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new
				{
					message = "Demo dolabı oluşturulamadı. Lütfen yeniden deneyin.",
					code = "DEMO_BOOTSTRAP_FAILED"
				});
			}
		}
	}
}
