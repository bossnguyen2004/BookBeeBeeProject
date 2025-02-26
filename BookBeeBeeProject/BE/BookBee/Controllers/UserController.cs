using BookBee.DTO.User;
using BookBee.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		[HttpGet("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> GetUserById(int id)
		{
			var res =await _userService.GetUserById(id);
			return StatusCode(res.Code, res);
		}

		[HttpGet("Personal")]
		public async Task<IActionResult> GetPersonalInfo()
		{
			var res =await _userService.GetPersonalInfo();
			return StatusCode(res.Code, res);
		}

		[HttpGet("{username}")]
		public async Task<IActionResult> GetUserByUsername(string username)
		{
			var res =await _userService.GetUserByUsername(username);
			return StatusCode(res.Code, res);
		}
		[HttpGet]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> GetUsers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var res =await _userService.GetUsers(page, pageSize, key, sortBy);
			return StatusCode(res.Code, res);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdateUser(int id, UserAccountDTO updateUserDTO)
		{
			var res = await _userService.UpdateUser(id, updateUserDTO);
			return StatusCode(res.Code, res);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var res = await _userService.DeleteUser(id);
			return StatusCode(res.Code, res);
		}
		[HttpPut("{id}/restore")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> RestoreUser(int id)
		{
			var res =await _userService.RestoreUser(id);
			return StatusCode(res.Code, res);
		}
		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreateUser(UserAccountDTO createUserDTO)
		{
			var res =await _userService.CreateUser(createUserDTO);
			return StatusCode(res.Code, res);
		}
	}
}
