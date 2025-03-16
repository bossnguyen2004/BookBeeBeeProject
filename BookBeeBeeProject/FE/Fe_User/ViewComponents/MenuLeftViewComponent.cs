using Fe_Admin.Models;
using Fe_User.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace Fe_User.ViewComponents
{
    public class MenuLeftViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MenuLeftViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var url = $"https://localhost:7287/api/Tag?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<Fe_Admin.Models.Tag>>>(jsonString);

            if (result?.Data == null || result.Data.Count == 0)
            {
                return View("Error");
            }

            var tags = result.Data.Select(tag => new Fe_User.Models.Tag
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList();

            var totalItems = result.Total > 0 ? result.Total : tags.Count;
            var pagedList = new StaticPagedList<Fe_User.Models.Tag>(tags, currentPage, pageSize, totalItems);
            ViewBag.SearchKey = key;

            return View("Default", pagedList);
        }
    }
}
