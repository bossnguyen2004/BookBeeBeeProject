using BookBee.DTO.Address;
using BookBee.Services.AddressService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class AddressController : ControllerBase
	{
		private readonly IAddressService _addressService;
		public AddressController(IAddressService addressService)
		{
			_addressService = addressService;
		}
		[HttpPost]
		public async Task<IActionResult> CreateAddress(AddressDTO createAddressDTO)
		{
			var res = await _addressService.CreateAddress(createAddressDTO);
			return StatusCode(res.Code, res);
		}
		[HttpPost("User/Self")]
		public async Task<IActionResult> SelfCreateAddress(AddressDTO selfCreateAddressDTO)
		{
			var res =await _addressService.SelfCreateAddress(selfCreateAddressDTO);
			return StatusCode(res.Code, res);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAddressById(int id)
		{
			var res =await _addressService.GetAddressById(id);
			return StatusCode(res.Code, res);
		}
		[HttpGet("User/{id}")]
		public async Task<IActionResult> GetAddressByUser(int id)
		{
			var res = await _addressService.GetAddressByUser(id);
			return StatusCode(res.Code, res);
		}

		[HttpGet("User/Self")]
		public async Task<IActionResult> GetSelfAddresses()
		{
			var res =await _addressService.GetSelfAddresses();
			return StatusCode(res.Code, res);
		}
		[HttpGet]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var res =await _addressService.GetAddresses(page, pageSize, key, sortBy);
			return StatusCode(res.Code, res);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAddress(int id, AddressDTO updateAddressDTO)
		{
			var res =await _addressService.UpdateAddress(id, updateAddressDTO);
			return StatusCode(res.Code, res);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAddress(int id)
		{
			var res =await _addressService.DeleteAddress(id);
			return StatusCode(res.Code, res);
		}
	}
}
