using BookBee.DTO.Supplier;
using BookBee.Services.SupplierService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class SupplierController : ControllerBase
	{
		private readonly ISupplierService _supplierService;
		public SupplierController(ISupplierService supplierService)
		{
			_supplierService = supplierService;
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetSuppliersById(int id)
		{
			var res = await _supplierService.GetSuppliersById(id);
			return StatusCode(res.Code, res);
		}
		[HttpGet]
		public async Task<IActionResult> GetSuppliers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var res = await _supplierService.GetSuppliers(page, pageSize, key, sortBy);
			return StatusCode(res.Code, res);
		}
		[HttpPut("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdateSuppliers(int id, [FromBody] SupplierDTO supplierDTO)
		{
			var res = await _supplierService.UpdateSuppliers(id, supplierDTO);
			return StatusCode(res.Code, res);
		}
		[HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteSuppliers(int id)
		{
			var res = await _supplierService.DeleteSuppliers(id);
			return StatusCode(res.Code, res);
		}
		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreateSuppliers([FromBody] SupplierDTO supplierDTO)
		{
			var res = await _supplierService.CreateSuppliers(supplierDTO.Name);
			return StatusCode(res.Code, res);
		}
	}
}
