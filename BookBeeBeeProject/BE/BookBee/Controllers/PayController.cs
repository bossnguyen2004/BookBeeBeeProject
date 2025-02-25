using BookBee.DTO.PaymentMethod;
using BookBee.Services.PaymentMethodService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class PayController : ControllerBase
	{
		private readonly IPaymentMethodService _paymentMethodService;
		public PayController(IPaymentMethodService paymentMethodService)
		{
			_paymentMethodService = paymentMethodService;
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPaymentMethodById(int id)
		{
			var res = await _paymentMethodService.GetPaymentMethodById(id);
			return StatusCode(res.Code, res);
		}
		[HttpGet]
		public async Task<IActionResult> GetPaymentMethods(int? page = 1, int? pageSize = 5, string? key = "", string? sortBy = "ID")
		{
			var res = await _paymentMethodService.GetPaymentMethods(page, pageSize, key, sortBy);
			return StatusCode(res.Code, res);

		}
		[HttpPut("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethodDTO paymentMethodDTO)
		{
			var res = await _paymentMethodService.UpdatePaymentMethod(id, paymentMethodDTO);
			return StatusCode(res.Code, res);
		}
		[HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeletePaymentMethod(int id)
		{
			var res = await _paymentMethodService.DeletePaymentMethod(id);
			return StatusCode(res.Code, res);
		}

		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethodDTO paymentMethodDTO)
		{
			var res = await _paymentMethodService.CreatePaymentMethod(paymentMethodDTO);
			return StatusCode(res.Code, res);
		}
	}
}
