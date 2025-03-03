using Fe_Admin.DTO.Supplier;
using Fe_Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using X.PagedList.Extensions;

namespace Fe_Admin.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        public SupplierController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Supplier?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var supplier = json?["data"]?.ToObject<List<Supplier>>() ?? new List<Supplier>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(supplier.ToPagedList(currentPage, pageSize));
        }

        public async Task<IActionResult> Create(SupplierDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var url = "https://localhost:7287/api/Supplier";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);

            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Tạo nhà cung cấp mới thành công!"
                : "Không thể tạo nhà cung cấp mới.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }



        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID nhà cung cấp không hợp lệ.";
                return RedirectToAction("Index");
            }

            var url = $"https://localhost:7287/api/Supplier/{id}";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lấy thông tin nhà cung cấp . Mã lỗi: {response.StatusCode}";
                    return RedirectToAction("Index");
                }

                var supplier = JsonConvert.DeserializeObject<SupplierDTO>(await response.Content.ReadAsStringAsync());

                if (supplier == null)
                {
                    TempData["ErrorMessage"] = "Không thể tìm thấy nhà cung cấp.";
                    return RedirectToAction("Index");
                }

                return View(supplier);
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
                var url = $"https://localhost:7287/api/Supplier/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa nhà cung cấp." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
    }
}
