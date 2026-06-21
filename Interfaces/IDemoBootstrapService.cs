using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IDemoBootstrapService
	{
		Task<DemoBootstrapResponse> BootstrapAsync(DemoBootstrapRequest request);
	}
}
