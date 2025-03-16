using Fe_User.DTO;
using Fe_User.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using X.PagedList.Extensions;
using X.PagedList;
using System.Collections.Generic;

namespace Fe_User.Controllers
{
	public class MenuController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public MenuController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}



        [HttpGet]
        public async Task<IActionResult> SachMoi(int currentPage = 1, int pageSize = 10, string key = "", bool isPartial = false)
        {

			var url = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
			var client = _httpClientFactory.CreateClient();

			// Kiểm tra URL gọi API
			Console.WriteLine($"[API Call] URL: {url}");

			var response = await client.GetAsync(url);
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine($"[API Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
				return View("Error");
			}

			var jsonString = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"[API Response] JSON từ API: {jsonString}");

			try
			{
                var result = JsonConvert.DeserializeObject<ApiResponse<List<BookDTO>>>(jsonString);

                if (result?.Data == null || result.Data.Count == 0)
                {
                    Console.WriteLine("[ERROR] Không có sách nào được deserialize!");
                }
                else
                {
                    Console.WriteLine($"[SUCCESS] Số lượng sách nhận được: {result.Data.Count}");
                }

              
                var books = result.Data;

				var totalItems = result.Total > 0 ? result.Total : books.Count;

				Console.WriteLine($"[Parsed Data] Tổng số sách từ API: {totalItems}");
				Console.WriteLine($"[Parsed Data] Số lượng sách trong danh sách books: {books.Count}");

				// Phân trang
				var pagedList = new StaticPagedList<BookDTO>(books.AsEnumerable(), currentPage, pageSize, totalItems);

				Console.WriteLine($"[PagedList] Tổng số sách: {pagedList.TotalItemCount}, Trang hiện tại: {currentPage}, PageSize: {pageSize}");

				ViewBag.SearchKey = key;

				if (isPartial)
				{
					return PartialView("_BookListPartial", pagedList);
				}

				return View(pagedList);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] Lỗi parse JSON: {ex.Message}");
				return View("Error");
			}

		}


		[HttpGet]
		public async Task<IActionResult> DanhMuc(int currentPage = 1, int pageSize = 10, string key = "", bool isPartial = false)
		{
            var url = $"https://localhost:7287/api/Tag?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var client = _httpClientFactory.CreateClient();

            // Kiểm tra URL gọi API
            Console.WriteLine($"[API Call] URL: {url}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[API Response] JSON từ API: {jsonString}");

            try
            {
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Tag>>>(jsonString);

                if (result?.Data == null || result.Data.Count == 0)
                {
                    Console.WriteLine("[ERROR] Không có thể loại nào được deserialize!");
                }
                else
                {
                    Console.WriteLine($"[SUCCESS] Số lượng thể loại nhận được: {result.Data.Count}");
                }


                var books = result.Data;

                var totalItems = result.Total > 0 ? result.Total : books.Count;

                Console.WriteLine($"[Parsed Data] Tổng số thể loại từ API: {totalItems}");
                Console.WriteLine($"[Parsed Data] Số lượng thể loại trong danh thể loại: {books.Count}");

                // Phân trang
                var pagedList = new StaticPagedList<Tag>(books.AsEnumerable(), currentPage, pageSize, totalItems);

                Console.WriteLine($"[PagedList] Tổng số thể loại: {pagedList.TotalItemCount}, Trang hiện tại: {currentPage}, PageSize: {pageSize}");

                ViewBag.SearchKey = key;

                if (isPartial)
                {
                    return PartialView("_DanhMuc", pagedList);
                }

                return View(pagedList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Lỗi parse JSON: {ex.Message}");
                return View("Error");
            }


        }





        [HttpGet]
        public async Task<IActionResult> MenuLeft(int currentPage = 1, int pageSize = 10, string key = "", bool isPartial = false)
        {
            var url = $"https://localhost:7287/api/Tag?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var client = _httpClientFactory.CreateClient();

            // Kiểm tra URL gọi API
            Console.WriteLine($"[API Call] URL: {url}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[API Response] JSON từ API: {jsonString}");

            try
            {
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Fe_Admin.Models.Tag>>>(jsonString);

                if (result?.Data == null || result.Data.Count == 0)
                {
                    Console.WriteLine("[ERROR] Không có thể loại nào được deserialize!");
                }
                else
                {
                    Console.WriteLine($"[SUCCESS] Số lượng thể loại nhận được: {result.Data.Count}");
                }

                // Chuyển đổi danh sách từ Fe_Admin.Models.Tag sang Fe_User.Models.Tag nếu cần
                var books = result.Data.Select(tag => new Fe_User.Models.Tag
                {
                    Id = tag.Id,
                    Name = tag.Name
                }).ToList();

                var totalItems = result.Total > 0 ? result.Total : books.Count;
                Console.WriteLine($"[Parsed Data] Tổng số thể loại từ API: {totalItems}");

                // Tạo danh sách phân trang
                var pagedList = new StaticPagedList<Fe_User.Models.Tag>(books, currentPage, pageSize, totalItems);

                Console.WriteLine($"[PagedList] Tổng số thể loại: {pagedList.TotalItemCount}, Trang hiện tại: {currentPage}, PageSize: {pageSize}");

                ViewBag.SearchKey = key;

                if (isPartial)
                {
                    return PartialView("_MenuLeft", pagedList);
                }

                return View(pagedList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Lỗi parse JSON: {ex.Message}");
                return View("Error");
            }



        }


































    }

}
