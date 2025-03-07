using Fe_Admin.DTO;
using Fe_Admin.DTO.Author;
using Fe_Admin.DTO.Book;
using Fe_Admin.DTO.Publisher;
using Fe_Admin.DTO.Supplier;
using Fe_Admin.DTO.Tag;
using Fe_Admin.DTO.Voucher;
using Fe_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using X.PagedList.Extensions;

namespace Fe_Admin.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ILogger<BookController> _logger;
        public BookController(IHttpClientFactory httpClientFactory, ITempDataProvider tempDataProvider, ILogger<BookController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tempDataProvider = tempDataProvider;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Book?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";
            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var tag = json?["data"]?.ToObject<List<Book>>() ?? new List<Book>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(tag.ToPagedList(currentPage, pageSize));
        }


        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var tagsResponse = await client.GetAsync("https://localhost:7287/api/Tag");
                var publishersResponse = await client.GetAsync("https://localhost:7287/api/Publisher");
                var authorsResponse = await client.GetAsync("https://localhost:7287/api/Author");
                var vouchersResponse = await client.GetAsync("https://localhost:7287/api/Voucher");
                var supplierResponse = await client.GetAsync("https://localhost:7287/api/Supplier");



                if (!tagsResponse.IsSuccessStatusCode || !supplierResponse.IsSuccessStatusCode || !publishersResponse.IsSuccessStatusCode || !authorsResponse.IsSuccessStatusCode || !vouchersResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Không thể tải danh sách từ API.";
                    throw new Exception("Không thể tải danh sách Tags, Publishers,Supplier , Authors và Vouchers từ API.");
                }

                var tagResult = JsonConvert.DeserializeObject<Response<List<TagDTO>>>(await tagsResponse.Content.ReadAsStringAsync());
                var publisherResult = JsonConvert.DeserializeObject<Response<List<PublisherDTO>>>(await publishersResponse.Content.ReadAsStringAsync());
                var authorResult = JsonConvert.DeserializeObject<Response<List<AuthorDTO>>>(await authorsResponse.Content.ReadAsStringAsync());
                var supplierResult = JsonConvert.DeserializeObject<Response<List<SupplierDTO>>>(await supplierResponse.Content.ReadAsStringAsync());
                var voucherResult = JsonConvert.DeserializeObject<Response<List<VoucherDTO>>>(await vouchersResponse.Content.ReadAsStringAsync());

                if (tagResult?.Data == null || publisherResult?.Data == null || authorResult?.Data == null || voucherResult?.Data == null || supplierResult?.Data== null)
                {
                    TempData["ErrorMessage"] = "Dữ liệu từ API không hợp lệ.";
                    throw new Exception("Dữ liệu từ API không hợp lệ.");
                }

                ViewBag.Tags = tagResult.Data;
                ViewBag.Publishers = publisherResult.Data;
                ViewBag.Authors = authorResult.Data;
                ViewBag.Vouchers = voucherResult.Data;
                ViewBag.Supplier = supplierResult.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu: {ex.Message}");

                ViewBag.Tags = new List<TagDTO>();
                ViewBag.Publishers = new List<PublisherDTO>();
                ViewBag.Authors = new List<AuthorDTO>();
                ViewBag.Vouchers = new List<VoucherDTO>();
                ViewBag.Supplier = new List<SupplierDTO>();
               TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "Không thể tải dữ liệu. Vui lòng thử lại sau.";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] BookDTO model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Tạo sách mới thành công!";
                return Json(new { code = 200, message = "Tạo sách mới thành công!", redirectUrl = Url.Action("Index") });
            }

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Title), "Title");
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.NumberOfPages.ToString()), "NumberOfPages");
            content.Add(new StringContent(model.PublishDate.ToString("yyyy-MM-dd")), "PublishDate");
            content.Add(new StringContent(model.Language), "Language");
            content.Add(new StringContent(model.Count.ToString()), "Count");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.GiaNhap.ToString()), "GiaNhap");
            content.Add(new StringContent(model.GiaThucTe.ToString()), "GiaThucTe");
            content.Add(new StringContent(model.Status.ToString()), "Status");
            content.Add(new StringContent(model.Format ?? ""), "Format");
            content.Add(new StringContent(model.PageSize ?? ""), "PageSize");
            content.Add(new StringContent(model.IsDeleted.ToString()), "IsDeleted");
            content.Add(new StringContent(model.PublisherId.ToString()), "PublisherId");
            content.Add(new StringContent(model.TagIds.ToString()), "TagIds");
            if (model.VoucherIds.HasValue)
            {
                content.Add(new StringContent(model.VoucherIds.ToString()), "VoucherIds");
            }
            content.Add(new StringContent(model.AuthorId.ToString()), "AuthorId");
            content.Add(new StringContent(model.SupplierId.ToString()), "SupplierId");
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageStream = imageFile.OpenReadStream();
                var fileContent = new StreamContent(imageStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(fileContent, "imageFile", imageFile.FileName);
            }

            var url = "https://localhost:7287/api/Book";
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo sách mới thành công!";
                    return Json(new { code = 200, message = "Tạo sách mới thành công!", redirectUrl = Url.Action("Index") });
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Không thể tạo sách mới. Lỗi: {errorResponse}";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi kết nối API: {ex.Message}";
                return View(model);
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var url = $"https://localhost:7287/api/Book/{id}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa Sách." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }



        public async Task<IActionResult> Inactive(int currentPage = 1, int pageSize = 10, string key = "")
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var url = $"https://localhost:7287/api/Book/inactive?page={currentPage}&pageSize={pageSize}{(string.IsNullOrEmpty(key) ? "" : $"&key={Uri.EscapeDataString(key)}")}";

            var response = await _httpClientFactory.CreateClient().GetAsync(url);
            if (!response.IsSuccessStatusCode) return View("Error", "Không thể truy cập dữ liệu từ API.");

            var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var books = json?["data"]?.ToObject<List<Book>>() ?? new List<Book>();
            var totalPages = (int)Math.Ceiling((double)(json?["total"]?.ToObject<int>() ?? 0) / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Searchtext = key;

            return View(books.ToPagedList(currentPage, pageSize));

        }




















    }
}
