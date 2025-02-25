using Fe_User.DTO.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Fe_User.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {

            var accessToken = HttpContext.Session.GetString("AccessToken");
            var accessRole = HttpContext.Session.GetString("Result");

            if (string.IsNullOrEmpty(accessToken) || (accessRole != "user"))
            {
                return RedirectToAction("Login");
            }

            return View();

        }

        [HttpGet]
        public IActionResult Login()
        {

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {

            if (!ModelState.IsValid)
            {
                return View(loginDTO);
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/account/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);

                // Lưu trữ token và role vào session
                HttpContext.Session.SetString("AccessToken", loginResponse.data.token);
                HttpContext.Session.SetString("Result", loginResponse.data.role.name);

                // Kiểm tra quyền truy cập của người dùng
                if (loginResponse.data.role.name == "user")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Bạn không có quyền truy cập!");
                    return View(loginDTO);
                }
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Đăng nhập thất bại: {errorResponse}");
                return View(loginDTO);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            HttpContext.Session.Remove("AccessToken");
            HttpContext.Session.Remove("Result");
            HttpContext.Session.Remove("JWTToken");

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


		[HttpGet]
		public IActionResult Register()
		{
			var accessToken = HttpContext.Session.GetString("AccessToken");
			if (!string.IsNullOrEmpty(accessToken))
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDTO);
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            var jsonContent = JsonConvert.SerializeObject(registerDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/account/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Đăng ký thất bại: {errorResponse}");
                return View(registerDTO);
            }
        }


    }
}
