using BookBee.DTO.OrderDetail;
using BookBee.Model;
using BookBee.Services.TaiQuayServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TaiQuayController : ControllerBase
    {
        private readonly ITaiQuayServices _taiQuayServices;
        public TaiQuayController(ITaiQuayServices taiQuayServices)
        {
            _taiQuayServices= taiQuayServices;
        }

        [HttpGet("GetAllHdTaiQuay")]
        public async Task<IActionResult> GetAllHdTaiQuay()
        {
            var result = _taiQuayServices.GetAllHDTaiQuay();
            return Ok(result);
        }

        [HttpPost("CreateHdTaiQuay")]
        public async  Task<IActionResult> CreateHdTaiQuay([FromBody] HDTaiQuayDTO _requestHdTaiQuay)
        {
            var result = _taiQuayServices.TaoHoaDonTaiQuay(_requestHdTaiQuay);
            return Ok(result);
        }



        [HttpPost("AddBillDetail")]
        public async Task<IActionResult> AddBillDetail(string mahoadon, string codeProductDetail, int? soluong)
        {
            var result = await _taiQuayServices.AddBillDetail(mahoadon, codeProductDetail, soluong);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPut("UpdateBillDetail")]
        public IActionResult UpdateBillDetail(string mahoadon, string codeProductDetail, int soluong)
        {
            var result = _taiQuayServices.CapNhatSoLuongHoaDonChiTietTaiQuay(mahoadon, codeProductDetail, soluong);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("TruQuantityBillDetail")]
        public IActionResult TruQuantityBillDetail(int idBillDetail)
        {
            var result = _taiQuayServices.TruQuantityBillDetail(idBillDetail).Result;
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("CongQuantityBillDetail")]
        public IActionResult CongQuantityBillDetail(int idBillDetail)
        {
            var result = _taiQuayServices.CongQuantityBillDetail(idBillDetail).Result;
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPut("ThanhToanTaiQuay")]
        public IActionResult ThanhToanTaiQuay(Order _hoaDon)
        {
            var result = _taiQuayServices.ThanhToan(_hoaDon);
            if (!result)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpDelete("XoaSanPhamKhoiHoaDon")]
        public IActionResult XoaSanPhamKhoiHoaDon(string maHD, string maSP)
        {
            var result = _taiQuayServices.XoaSanPhamKhoiHoaDon(maHD, maSP);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPut("HuyHoaDon")]
        public IActionResult HuyHoaDon(string maHD, string lyDoHuy)
        {
            var result = _taiQuayServices.HuyHoaDonAsync(maHD, lyDoHuy).Result;
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
