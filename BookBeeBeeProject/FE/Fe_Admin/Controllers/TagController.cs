using Fe_Admin.DTO.Tag;
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
    public class TagController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        public TagController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Tag?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var tag = json?["data"]?.ToObject<List<Tag>>() ?? new List<Tag>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(tag.ToPagedList(currentPage, pageSize));
        }

        public async Task<IActionResult> Create(TagDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var url = "https://localhost:7287/api/Tag";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);

            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Tạo danh mục thể loại mới thành công!"
                : "Không thể tạo danh mục thể loại mới.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }




        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID danh mục thể loại không hợp lệ.";
                return RedirectToAction("Index");
            }

            var url = $"https://localhost:7287/api/Tag/{id}";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lấy thông tin danh mục thể loại . Mã lỗi: {response.StatusCode}";
                    return RedirectToAction("Index");
                }

                var publisher = JsonConvert.DeserializeObject<TagDTO>(await response.Content.ReadAsStringAsync());

                if (publisher == null)
                {
                    TempData["ErrorMessage"] = "Không thể tìm thấy danh mục thể loại.";
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
                var url = $"https://localhost:7287/api/Tag/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa danh mục thể loại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

    }
}
