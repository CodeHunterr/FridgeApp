using FridgeApp.Entities;
using FridgeApp.Models;

namespace FridgeApp.Interfaces
{
	public interface IUserService
	{
		Task<User> CreateUser(CreateUserRequest userRequest);
		Task<List<User>> GetUsers();
	}
}
