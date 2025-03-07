using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Fe_Admin.Models;
using X.PagedList.Extensions;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Fe_Admin.DTO.OrderVoucher;

namespace Fe_Admin.Controllers
{
    public class OrderVoucherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;

        public OrderVoucherController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }
        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/OrderVoucher?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var vouchers = json?["data"]?.ToObject<List<OrderVoucher>>() ?? new List<OrderVoucher>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(vouchers.ToPagedList(currentPage, pageSize));
        }




        public async Task<IActionResult> Create(OrderVoucherDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var url = "https://localhost:7287/api/OrderVoucher";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);

            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Tạo OrderVoucher mới thành công!"
                : "Không thể tạo OrderVoucher mới.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }





        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID voucher không hợp lệ.";
                return RedirectToAction("Index");
            }

            var url = $"https://localhost:7287/api/OrderVoucher/{id}";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lấy thông tin voucher . Mã lỗi: {response.StatusCode}";
                    return RedirectToAction("Index");
                }

                var publisher = JsonConvert.DeserializeObject<OrderVoucherDTO>(await response.Content.ReadAsStringAsync());

                if (publisher == null)
                {
                    TempData["ErrorMessage"] = "Không thể tìm thấy voucher.";
                    return RedirectToAction("Index");
                }

                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var url = $"https://localhost:7287/api/OrderVoucher/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa voucher." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }









    }
}
