using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/demo/bootstrap")]
	public class DemoBootstrapController : ControllerBase
	{
		private readonly IDemoBootstrapService _demoBootstrapService;

		public DemoBootstrapController(IDemoBootstrapService demoBootstrapService)
		{
			_demoBootstrapService = demoBootstrapService;
		}

		[HttpPost]
		public async Task<ActionResult<DemoBootstrapResponse>> Bootstrap(
			[FromBody] DemoBootstrapRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.InstallId))
			{
				return BadRequest(new { message = "Kurulum kimliği gerekli." });
			}

			var response = await _demoBootstrapService.BootstrapAsync(request);
			return Ok(response);
		}
	}
}
