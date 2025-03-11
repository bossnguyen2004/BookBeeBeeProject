using Fe_User.DTO;
using Fe_User.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using X.PagedList.Extensions;
using X.PagedList;

namespace Fe_User.Controllers
{
	public class MenuController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public MenuController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> SachMoi(int currentPage = 1, int pageSize = 10, string key = "", bool isPartial = false)
	    {
			var url = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}&key={Uri.EscapeDataString(key)}";

			var response = await _httpClientFactory.CreateClient().GetAsync(url);

			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine($"Lỗi API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
				return View("Error");
			}

			var jsonString = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Dữ liệu từ API: {jsonString}"); // Kiểm tra dữ liệu nhận từ API

			var json = JObject.Parse(jsonString);
			var books = json["data"]?.ToObject<List<Book>>() ?? new List<Book>();
			var totalItems = json["total"]?.Value<int>() ?? 0;

			var pagedList = new StaticPagedList<Book>(books, currentPage, pageSize, totalItems);

			if (isPartial)
			{
				return PartialView("_BookListPartial", pagedList);
			}

			return View(pagedList);

		}
	}
	
}
