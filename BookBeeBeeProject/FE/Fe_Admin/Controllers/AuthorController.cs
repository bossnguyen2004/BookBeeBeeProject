using Fe_Admin.DTO.Author;
using Fe_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using X.PagedList.Extensions;

namespace Fe_Admin.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        public AuthorController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
        }


        public  async Task<IActionResult> Index(int currentPage = 1, int pageSize = 5, string key = "")
        {

            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Author?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var authors = json?["data"]?.ToObject<List<Author>>() ?? new List<Author>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(authors.ToPagedList(currentPage, pageSize));
        }


        public async Task<IActionResult> Create(AuthorDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var url = "https://localhost:7287/api/Author";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);

            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Tạo tác giả mới thành công!"
                : "Không thể tạo tác giả mới.";

            return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(model);
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID tác giả không hợp lệ.";
                return RedirectToAction("Index");
            }

            var url = $"https://localhost:7287/api/Author/{id}";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lấy thông tin tác giả. Mã lỗi: {response.StatusCode}";
                    return RedirectToAction("Index");
                }

                var author = JsonConvert.DeserializeObject<AuthorDTO>(await response.Content.ReadAsStringAsync());

                if (author == null)
                {
                    TempData["ErrorMessage"] = "Không thể tìm thấy tác giả.";
                    return RedirectToAction("Index");
                }

                return View(author);
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
                var url = $"https://localhost:7287/api/Author/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa tác giả." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


    }
}
