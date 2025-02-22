using BookBee.DTO.Account;
using BookBee.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Login")]
        public ActionResult Login(LoginDTO loginDTO)
        {
            var res = _authService.Login(loginDTO.Username, loginDTO.Password);
            return StatusCode(res.Code, res);
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserDTO registerUserDTO)
        {
            var res = await _authService.Register(registerUserDTO);
            return StatusCode(res.Code, res);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var res = _authService.ChangePassword(changePasswordDTO);
            return StatusCode(res.Code, res);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] string email)
        {
            var res = await _authService.ForgotPassword(email);
            return StatusCode(res.Code, res);
        }

        [HttpPost("ResetPassword")]
        public ActionResult ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var res = _authService.ResetPassword(resetPasswordDTO);
            return StatusCode(res.Code, res);
        }
    }
}
