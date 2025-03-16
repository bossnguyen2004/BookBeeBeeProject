using Fe_User.DTO;
using Fe_User.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using X.PagedList;

namespace Fe_User.Controllers
{
	public class BookController : Controller
	{
        private readonly IHttpClientFactory _httpClientFactory;
        public BookController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "", bool isPartial = false, string category = "")
		{
            var url = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(key))
            {
                url += $"&key={Uri.EscapeDataString(key)}";
            }

            if (!string.IsNullOrEmpty(category))
            {
                url += $"&category={Uri.EscapeDataString(category)}";
            }

            var client = _httpClientFactory.CreateClient();
            Console.WriteLine($"[API Call] Request URL: {url}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[API Response] JSON: {jsonString}");

            try
            {
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Book>>>(jsonString);

                if (result == null)
                {
                    Console.WriteLine("[ERROR] API Response is NULL!");
                    return View("Error");
                }

                Console.WriteLine($"[DEBUG] Total Items: {result.Total}, Data Count: {result?.Data?.Count ?? 0}");

                ViewBag.SearchKey = key;
                ViewBag.SelectedCategory = category;


                Console.WriteLine($"[DEBUG] Tổng số sách trước khi lọc: {result.Data.Count}");

                if (!string.IsNullOrEmpty(category))
                {
                    Console.WriteLine($"[DEBUG] Lọc sách theo danh mục: {category}");

                    result.Data = result.Data.Where(b => b.Tags.Any(tag => tag.Id == Convert.ToInt32(category))).ToList();
                }

                Console.WriteLine($"[DEBUG] Số sách sau khi lọc: {result.Data.Count}");

                if (result.Data == null || !result.Data.Any())
                {
                    Console.WriteLine($"[INFO] Không có sách nào trong danh mục '{category}'!");
                    ViewBag.NoProductsMessage = $"Danh mục '{category}' chưa có sản phẩm nào.";

                    return View(new StaticPagedList<Book>(new List<Book>(), currentPage, pageSize, 0));
                }



                Console.WriteLine($"[SUCCESS] Số lượng sách nhận được: {result.Data.Count}");

                var pagedList = new StaticPagedList<Book>(result.Data, currentPage, pageSize, result.Total);

                if (isPartial)
                {
                    Console.WriteLine("[INFO] Returning Partial View.");
                    return PartialView("_BookListPartial", pagedList);
                }

                return View(pagedList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception: {ex.Message}");
                return View("Error");
            }
        }



        [HttpGet("chi-tiet/{slug}-p{id}")]
        public async Task<IActionResult> Detail(string slug, int id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7287/api/Book/{id}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var bookResponse = JsonConvert.DeserializeObject<ApiResponse<BookDTO>>(jsonString);

            return View(bookResponse.Data);
        }

















    }
    
}
