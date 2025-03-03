using Fe_Admin.DTO.Publisher;
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
    public class PublisherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        public PublisherController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Publisher?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var publisher = json?["data"]?.ToObject<List<Publisher>>() ?? new List<Publisher>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(publisher.ToPagedList(currentPage, pageSize));
        }


        public async Task<IActionResult> Create(PublisherDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var url = "https://localhost:7287/api/Publisher";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);

            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Tạo nhà xuất bản mới thành công!"
                : "Không thể tạo nhà xuất bản mới.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }



        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID nhà xuất bản không hợp lệ.";
                return RedirectToAction("Index");
            }

            var url = $"https://localhost:7287/api/Publisher/{id}";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lấy thông tin nhà xuất bản . Mã lỗi: {response.StatusCode}";
                    return RedirectToAction("Index");
                }

                var publisher = JsonConvert.DeserializeObject<PublisherDTO>(await response.Content.ReadAsStringAsync());

                if (publisher == null)
                {
                    TempData["ErrorMessage"] = "Không thể tìm thấy nhà xuất bản.";
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
                var url = $"https://localhost:7287/api/Publisher/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa nhà xuất bản." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
    }
}
