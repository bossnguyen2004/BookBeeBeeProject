using BookBee.DTO.OrderVoucher;
using BookBee.DTO.Voucher;
using BookBee.Services.OrderVoucherService;
using BookBee.Services.VoucherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class OrderVoucherController : ControllerBase
	{
		private readonly IOrderVoucherService _ordervoucherService;
		public OrderVoucherController(IOrderVoucherService ordervoucherService)
		{
			_ordervoucherService = ordervoucherService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderVoucherById(int id)
		{
			var res = await _ordervoucherService.GetOrderVoucherById(id);
			return StatusCode(res.Code, res);
		}
		[HttpGet]
		public async Task<IActionResult> GetOrderVoucher(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var res = await _ordervoucherService.GetOrderVoucher(page, pageSize, key, sortBy);
			return StatusCode(res.Code, res);
		}
		[HttpPut("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdateOrderVoucher(int id, OrderVoucherDTO ordervoucherRequestDto)
		{
			var res = await _ordervoucherService.UpdateOrderVoucher(id, ordervoucherRequestDto);
			return StatusCode(res.Code, res);
		}
		[HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteOrderVoucher(int id)
		{
			var res = await _ordervoucherService.DeleteOrderVoucher(id);
			return StatusCode(res.Code, res);
		}
		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreateOrderVoucher([FromBody] OrderVoucherDTO ordervoucherRequestDto)
		{

			var res = await _ordervoucherService.CreateOrderVoucher(ordervoucherRequestDto);
			return StatusCode(res.Code, res);
		}

		[HttpPatch("{id}/status")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> ChangeVoucherStatus(int id, [FromBody] OrderVoucherDTO ordervoucherRequestDto)
		{
			if (id <= 0 || ordervoucherRequestDto == null)
			{
				return BadRequest(new { code = 400, message = "Dữ liệu không hợp lệ." });
			}

			var result = await _ordervoucherService.ChangeOrderVoucherStatus(id, ordervoucherRequestDto.Status.Value);
			if (result.Code == 200)
			{
				return Ok(new { code = 200, message = "Cập nhật trạng thái thành công!" });
			}

			return StatusCode(500, new { code = 500, message = "Lỗi khi cập nhật trạng thái voucher." });
		}
	}
}
