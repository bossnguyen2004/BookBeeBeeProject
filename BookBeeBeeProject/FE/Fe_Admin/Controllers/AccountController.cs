using Fe_Admin.DTO.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Fe_Admin.Controllers
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

            if (string.IsNullOrEmpty(accessToken) || (accessRole != "admin" && accessRole != "nhanvien"))
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
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin!";
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

                HttpContext.Session.SetString("AccessToken", loginResponse.data.token);
                HttpContext.Session.SetString("Result", loginResponse.data.role.name);
                string token = loginResponse.data.token;
                string role = loginResponse.data.role.name;
                if (loginDTO.RememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(30)
                    };
                    Response.Cookies.Append("AccessToken", token, cookieOptions);
                    Response.Cookies.Append("UserRole", role, cookieOptions);
                }
                else
                {
                    HttpContext.Session.SetString("AccessToken", token);
                    HttpContext.Session.SetString("UserRole", role);
                }

                if (loginResponse.data.role.name == "admin" || loginResponse.data.role.name == "nhanvien")
                {
                    TempData["Success"] = "Đăng nhập thành công!";
                    TempData.Keep("Success");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền truy cập!";
                    return View(loginDTO);
                }
            }
            else
            {
                TempData["Error"] = "Sai tài khoản hoặc mật khẩu!";
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

            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("UserRole");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult ClearTempData()
        {
            TempData.Remove("Success");
            TempData.Remove("Error");
            return Ok();
        }
    }
}
