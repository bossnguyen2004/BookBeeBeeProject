using BookBee.DTO.Voucher;
using BookBee.Services.VoucherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            var res =await _voucherService.GetVoucherById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetVouchers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res =await _voucherService.GetVoucher(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVoucher(int id, VoucherDTO voucherRequestDto)
        {
            var res =await _voucherService.UpdateVoucher(id, voucherRequestDto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var res =await _voucherService.DeleteVoucher(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherDTO voucherRequestDto)
        {

            var res =await _voucherService.CreateVoucher(voucherRequestDto);
            return StatusCode(res.Code, res);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeVoucherStatus(int id, [FromBody] VoucherDTO voucherRequestDto)
        {
            if (id <= 0 || voucherRequestDto == null)
            {
                return BadRequest(new { code = 400, message = "Dữ liệu không hợp lệ." });
            }

            var result =await _voucherService.ChangeVoucherStatus(id, voucherRequestDto.Status.Value);
            if (result.Code == 200)
            {
                return Ok(new { code = 200, message = "Cập nhật trạng thái thành công!" });
            }

            return StatusCode(500, new { code = 500, message = "Lỗi khi cập nhật trạng thái voucher." });
        }
    }
}
