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
        public async Task<IActionResult> AddBillDetail([FromBody]  string mahoadon, string codeProductDetail, int? soluong)
        {
            var result = await _taiQuayServices.AddBillDetail(mahoadon, codeProductDetail, soluong);
            if (result == null)
            {
                return BadRequest("Không thể tạo chi tiết hóa đơn.");
            }
            return Ok(result);
        }


        [HttpPut("UpdateBillDetail")]
        public IActionResult UpdateBillDetail(string mahoadon, string codeProductDetail, int soluong)
        {
            var result = _taiQuayServices.CapNhatSoLuongHoaDonChiTietTaiQuay(mahoadon, codeProductDetail, soluong);
            
            return Ok(result);
        }

        [HttpPut("TruQuantityBillDetail")]
        public IActionResult TruQuantityBillDetail(int idBillDetail)
        {
            var result = _taiQuayServices.TruQuantityBillDetail(idBillDetail).Result;
           
            return Ok(result);
        }

        [HttpPut("CongQuantityBillDetail")]
        public IActionResult CongQuantityBillDetail(int idBillDetail)
        {
            var result = _taiQuayServices.CongQuantityBillDetail(idBillDetail).Result;
            
            return Ok(result);
        }


        [HttpPut("ThanhToanTaiQuay")]
        public IActionResult ThanhToanTaiQuay(Order _hoaDon)
        {
            var result = _taiQuayServices.ThanhToan(_hoaDon);
           
            return Ok(result);
        }


        [HttpDelete("XoaSanPhamKhoiHoaDon")]
        public IActionResult XoaSanPhamKhoiHoaDon(string maHD, string maSP)
        {
            var result = _taiQuayServices.XoaSanPhamKhoiHoaDon(maHD, maSP);
           
            return Ok(result);
        }


        [HttpPut("HuyHoaDon")]
        public async Task<IActionResult> HuyHoaDon([FromBody]  string maHD, string lyDoHuy)
        {
            var result = await _taiQuayServices.HuyHoaDonAsync(maHD, lyDoHuy);
            return Ok(result);
        }
    }
}
