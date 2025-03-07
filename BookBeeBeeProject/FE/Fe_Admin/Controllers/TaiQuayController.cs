using Fe_Admin.DTO;
using Fe_Admin.DTO.Order;
using Fe_Admin.DTO.OrderDTO;
using Fe_Admin.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using X.PagedList.Extensions;

namespace Fe_Admin.Controllers
{
    public class TaiQuayController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        public TaiQuayController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }
        public async Task<IActionResult> DanhSachHoaDonCho(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var urlHoaDon = "https://localhost:7287/api/TaiQuay/GetAllHdTaiQuay";
                var responseHoaDon = await client.GetAsync(urlHoaDon);

                if (!responseHoaDon.IsSuccessStatusCode)
                {
                    var errorMessage = await responseHoaDon.Content.ReadAsStringAsync();
                    return View("Error", $"Không thể truy cập dữ liệu từ API. Mã lỗi: {responseHoaDon.StatusCode}, Thông tin lỗi: {errorMessage}");
                }

                var jsonHoaDon = await responseHoaDon.Content.ReadAsStringAsync();
                var danhSachHoaDon = JsonConvert.DeserializeObject<List<HoaDonChoDTO>>(jsonHoaDon) ?? new List<HoaDonChoDTO>();

                var urlSanPham = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}&key={Uri.EscapeDataString(key)}";
                var responseSanPham = await client.GetAsync(urlSanPham);

                if (!responseSanPham.IsSuccessStatusCode)
                {
                    return View("Error", "Không thể truy cập dữ liệu từ API sách.");
                }

                var jsonSanPham = await responseSanPham.Content.ReadAsStringAsync();
                var sanPhamObject = JsonConvert.DeserializeObject<JObject>(jsonSanPham);
                var danhSachSanPham = sanPhamObject?["data"]?.ToObject<List<Book>>() ?? new List<Book>();

                foreach (var hoaDon in danhSachHoaDon)
                {
                    foreach (var chiTiet in hoaDon.OrderDetailDTOs)
                    {
                        var sanpham = danhSachSanPham.FirstOrDefault(c => c.Id == chiTiet.Id);
                        if (sanpham != null)
                        {
                            chiTiet.CodeBook = $"{sanpham.Title}/{sanpham.AuthorId}";
                        }
                    }
                }

                var sortedList = danhSachHoaDon.OrderBy(c => Convert.ToInt32(c.OrderCode.Substring(4))).ToList();

                return View(sortedList);
            }
            catch (Exception ex)
            {
                return View("Error", $"Lỗi hệ thống: {ex.Message}");
            }

        }



        [HttpPost]
        public async Task<IActionResult> TaoHoaDonTaiQuay()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("🔒 Chưa đăng nhập! Chuyển hướng đến trang Login.");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var idUserString = HttpContext.Session.GetString("IdNguoiDung");

            if (string.IsNullOrEmpty(idUserString))
            {
                Console.WriteLine("⚠ Không tìm thấy UserAccountId trong session! Đang kiểm tra trong AccessToken...");
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);
                idUserString = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            }

            if (!int.TryParse(idUserString, out int idUser))
            {
                Console.WriteLine("❌ UserAccountId không hợp lệ hoặc không tồn tại!");
                return BadRequest(new { message = "User không hợp lệ!" });
            }
            try
            {
                var hoaDonMoi = new HoaDonChoDTO()
                {
                    IdNguoiDung = idUser
                };

                var jsonData = JsonConvert.SerializeObject(hoaDonMoi);
                Console.WriteLine($" Dữ liệu gửi lên API: {jsonData}");

                var urlHoaDon = "https://localhost:7287/api/TaiQuay/CreateHdTaiQuay";
                var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

                Console.WriteLine($"🔹 Request Headers:");
                foreach (var header in client.DefaultRequestHeaders)
                {
                    Console.WriteLine($"   - {header.Key}: {string.Join(", ", header.Value)}");
                }

                var response = await client.PostAsync(urlHoaDon, jsonContent);

                response.EnsureSuccessStatusCode();
                var jsonHoaDon = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<Response<HoaDonChoDTO>>(jsonHoaDon);

                if (responseData == null || responseData.Data == null)
                {
                    return Json(new { success = false, message = "Dữ liệu đơn hàng rỗng hoặc lỗi" });
                }

                return Json(new { success = true, orderCode = responseData.Data.OrderCode });
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🌐 Lỗi HTTP: {httpEx.Message}");
                return StatusCode(500, new { message = "Lỗi khi gọi API", details = httpEx.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Lỗi không xác định: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi hệ thống", details = ex.Message });
            }

        }


        [HttpGet]
        public async Task<IActionResult> HoaDonDuocChon(string maHD)
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (string.IsNullOrWhiteSpace(maHD))
            {
                return Ok(new
                {
                    TongTienGoc = 0,
                    TienPhaiTra = 0,
                    SoTienKhuyenMaiGiam = 0,
                    SoTienVoucherGiam = 0,
                    MaHoaDon = "",
                    NgayTao = "",
                    TenNhanVien = ""
                });
            }
            var urlHoaDon = "https://localhost:7287/api/TaiQuay/GetAllHdTaiQuay";
            var responseHoaDon = await client.GetAsync(urlHoaDon);

            if (!responseHoaDon.IsSuccessStatusCode)
            {
                var errorMessage = await responseHoaDon.Content.ReadAsStringAsync();
                return View("Error", $"Không thể truy cập dữ liệu từ API. Mã lỗi: {responseHoaDon.StatusCode}, Thông tin lỗi: {errorMessage}");
            }

            var jsonHoaDon = await responseHoaDon.Content.ReadAsStringAsync();
            var danhSachHoaDon = JsonConvert.DeserializeObject<List<HoaDonChoDTO>>(jsonHoaDon) ?? new List<HoaDonChoDTO>();

            var hoaDonDuocChon = danhSachHoaDon.FirstOrDefault(x => x.OrderCode == maHD);
            
            var user = HttpContext.Session.GetString("TenNguoiDung");

            if (hoaDonDuocChon == null)
            {
                return Ok(new
                {
                    TongTienGoc = 0,
                    TienPhaiTra = 0,
                    SoTienKhuyenMaiGiam = 0,
                    SoTienVoucherGiam = 0,
                    MaHoaDon = "Null",
                    NgayTao = "",
                    TenNhanVien = ""

                });
            }
            double tongTienGoc = 0;
            double tienPhaiTra = 0;
            string ngayTao = ((DateTime)hoaDonDuocChon.NgayTao).ToString("dd/MM/yyyy HH:mm");

            if (hoaDonDuocChon.OrderDetailDTOs.Count() > 0)
            {
                foreach (var item in hoaDonDuocChon.OrderDetailDTOs)
                {
                    tongTienGoc += (double)item.Price * (int)item.Quantity;
                    tienPhaiTra += (double)item.PriceBan * (int)item.Quantity;
                }
            }
            double soTienKhuyenMaiGiam = 0;
            if (tongTienGoc != tienPhaiTra)
            {
                soTienKhuyenMaiGiam = tongTienGoc - tienPhaiTra;
            }

            return Ok(new
            {
                TongTienGoc = tongTienGoc,
                TienPhaiTra = tienPhaiTra,
                SoTienKhuyenMaiGiam = soTienKhuyenMaiGiam,
                SoTienVoucherGiam = 0,
                MaHoaDon = hoaDonDuocChon.OrderCode,
                NgayTao = ngayTao,
                TenNhanVien = user
            });
        }



        [HttpGet]
        public async Task<IActionResult> ChiTietHoaDonDuocChon(string maHD,int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (string.IsNullOrWhiteSpace(maHD))
            {
                return PartialView("_HoaDonChiTietPartialView", new List<OrderDetailDTO>());
            }

            var urlHoaDon = "https://localhost:7287/api/TaiQuay/GetAllHdTaiQuay";
            var responseHoaDon = await client.GetAsync(urlHoaDon);
            if (!responseHoaDon.IsSuccessStatusCode)
            {
                var errorMessage = await responseHoaDon.Content.ReadAsStringAsync();
                return View("Error", $"Không thể truy cập dữ liệu từ API. Mã lỗi: {responseHoaDon.StatusCode}, Thông tin lỗi: {errorMessage}");
            }
            var jsonHoaDon = await responseHoaDon.Content.ReadAsStringAsync();
            var danhSachHoaDon = JsonConvert.DeserializeObject<List<HoaDonChoDTO>>(jsonHoaDon) ?? new List<HoaDonChoDTO>();


            var urlSanPham = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}&key={Uri.EscapeDataString(key)}";
            var responseSanPham = await client.GetAsync(urlSanPham);

            if (!responseSanPham.IsSuccessStatusCode)
            {
                return View("Error", "Không thể truy cập dữ liệu từ API sách.");
            }

            var jsonSanPham = await responseSanPham.Content.ReadAsStringAsync();
            var sanPhamObject = JsonConvert.DeserializeObject<JObject>(jsonSanPham);
            var danhSachSanPham = sanPhamObject?["data"]?.ToObject<List<Book>>() ?? new List<Book>();


            if (danhSachHoaDon.FirstOrDefault(x => x.OrderCode == maHD).OrderDetailDTOs != null)
            {
                var hoaDonDuocChon = danhSachHoaDon.FirstOrDefault(x => x.OrderCode == maHD).OrderDetailDTOs;
                foreach (var item2 in hoaDonDuocChon)
                {
                    var sanpham = danhSachSanPham.FirstOrDefault(c => c.Id == item2.BookId);
                    item2.TitleBook = sanpham.Title + "/" + sanpham.Author.Name ;
                    item2.CodeBook = sanpham.CodeBook;
                }
                return PartialView("_HoaDonChiTietPartialView", hoaDonDuocChon);
            }
            return PartialView("_HoaDonChiTietPartialView", null);

        }





        [HttpGet]
        public async Task<IActionResult> LoadPartialViewDanhSachSanPham(string tukhoa, int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Home");
            }
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
           
                var urlSanPham = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}&key={Uri.EscapeDataString(key)}";
                var responseSanPham = await client.GetAsync(urlSanPham);

                if (!responseSanPham.IsSuccessStatusCode)
                {
                    return View("Error", "Không thể truy cập dữ liệu từ API sách.");
                }

                var jsonSanPham = await responseSanPham.Content.ReadAsStringAsync();
                var sanPhamObject = JsonConvert.DeserializeObject<JObject>(jsonSanPham);
                var danhSachSanPham = sanPhamObject?["data"]?.ToObject<List<Book>>() ?? new List<Book>();

                if (!string.IsNullOrWhiteSpace(tukhoa))
                {
                    var listsanphamLoc = JsonConvert.DeserializeObject<List<Book>>(jsonSanPham).Where(c => c.Title.ToLower().Replace(" ", "").Contains(tukhoa.ToLower().Replace(" ", "")) || c.CodeBook.ToLower().Replace(" ", "").Contains(tukhoa.ToLower().Replace(" ", "")));

                    return PartialView("_DanhSachSanPhamPartialView", listsanphamLoc);

                }


                else
                    return PartialView("_DanhSachSanPhamPartialView", danhSachSanPham);
           
        }




        //[HttpPost]
        //public async Task<IActionResult> ThemSanPhamVaoHoaDon(string maHD, string idSanPham, int currentPage = 1, int pageSize = 10, string key = "")
        //{



        //    if (string.IsNullOrWhiteSpace(maHD))
        //    {
        //        return Ok(new { TrangThai = false, TrangThaiHang = false });
        //    }

        //    var accessToken = HttpContext.Session.GetString("AccessToken");
        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    var client = _httpClientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    var urlSanPham = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}&key={Uri.EscapeDataString(key)}";
        //    var responseSanPham = await client.GetAsync(urlSanPham);

        //    if (!responseSanPham.IsSuccessStatusCode)
        //    {
        //        return View("Error", "Không thể truy cập dữ liệu từ API sách.");
        //    }

        //    var jsonSanPham = await responseSanPham.Content.ReadAsStringAsync();
        //    var sanPhamObject = JsonConvert.DeserializeObject<JObject>(jsonSanPham);
        //    var danhSachSanPham = sanPhamObject?["data"]?.ToObject<List<Book>>() ?? new List<Book>().FirstOrDefault(sp => sp.Id==Convert.ToInt32(idSanPham) );


        //    if (danhSachSanPham.SoLuongTon == 0)
        //    {
        //        return Ok(new { TrangThai = false, TrangThaiHang = true });
        //    }


        //    var urlAddBillDetail = $"api/BanHangTaiQuay/AddBillDetail?mahoadon={maHD}&codeProductDetail={sanPham.MaSanPhamChiTiet}";
        //    var responseAddBillDetail = await client.PostAsync(urlAddBillDetail, null);
        //    var jsonResponse = await responseAddBillDetail.Content.ReadAsStringAsync();
        //    var responseDto = JsonConvert.DeserializeObject<ResponseDto>(jsonResponse);
        //    var message = responseDto.Message;
        //    var responseHDCT = JsonConvert.DeserializeObject<HoaDonChiTietDto>(responseDto.Content.ToString());


        //    var url = $"/api/BanHangTaiQuay/GetAllHdTaiQuay";
        //    var respone = client.GetAsync(url).Result;
        //    string data = await respone.Content.ReadAsStringAsync();
        //    var hoaDon = JsonConvert.DeserializeObject<List<HoaDonChoDTO>>(data.ToString());
        //    var hoaDonDuocChon = hoaDon.FirstOrDefault(x => x.MaHoaDon == maHD).hoaDonChiTietDTOs.FirstOrDefault(x => x.Id == responseHDCT.Id);

        //    var tongTienThayDoi = hoaDonDuocChon.Price;//gias thuc te trong spct
        //    var soTienTraLaiThayDoi = hoaDonDuocChon.PriceBan;// gia ban trog hoa don
        //    var SoTienKhuyenMaiGiam = tongTienThayDoi - soTienTraLaiThayDoi;
        //    return Ok(new
        //    {
        //        TrangThai = true,
        //        IdHoaDon = hoaDonDuocChon.HoaDonId,
        //        IdHoaDonChiTiet = hoaDonDuocChon.Id,
        //        IdSanPhamChiTiet = hoaDonDuocChon.SanPhamChiTietId,
        //        SoLuong = hoaDonDuocChon.Quantity,
        //        GiaBan = hoaDonDuocChon.PriceBan,
        //        GiaGoc = hoaDonDuocChon.Price,
        //        TenSanPham = sanPham.TenSanPham + "/" + sanPham.TenMauSac + "/" + sanPham.TenThuongHieu,
        //        MaSanPham = sanPham.MaSanPham,
        //        TongTienThayDoi = tongTienThayDoi,
        //        SoTienTraLaiThayDoi = soTienTraLaiThayDoi,
        //        SoTienKhuyenMaiGiam = SoTienKhuyenMaiGiam,
        //        SoTienVoucherGiam = 0,
        //    });
        //}








    }
}
