using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgeApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateUserRequest userRequest)
		{
			var user = await _userService.CreateUser(userRequest);

			return Ok(user);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var users = await _userService.GetUsers();

			return Ok(users);
		}
	}
}
