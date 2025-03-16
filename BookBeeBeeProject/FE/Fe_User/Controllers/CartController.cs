using Fe_Admin.Models;
using Fe_User.DTO;
using Fe_User.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Fe_User.Controllers
{
    public class CartController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7287/api";
        private readonly IHttpClientFactory _httpClientFactory;
        private const string GuestCartCookieName = "GuestCartId";
        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? userId, string guestCartId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{_apiBaseUrl}/cart?userId={userId}&guestCartId={guestCartId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cartItems = JsonConvert.DeserializeObject<List<Cart>>(content);
                return View(cartItems);
            }

            return View(new List<Cart>());

        }
        //[HttpPost]
        //public async Task<IActionResult> AddToCart(int bookId, int quantity)
        //{
           
        //}

       
    }
}
