using FridgeApp.Data;
using FridgeApp.Entities;
using FridgeApp.Interfaces;
using FridgeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Services
{
	public class UserService : IUserService
	{
		private readonly AppDbContext _context;

		public UserService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<User> CreateUser(CreateUserRequest userRequest)
		{
			var user = new User
			{
				Email = userRequest.Email,
				PasswordHash = userRequest.PasswordHash,
				CreatedDate = DateTime.Now
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return user;
		}

		public async Task<List<User>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}
	}
}
