using BookBee.DTO.Account;
using BookBee.DTO.Employee;
using BookBee.DTO.Response;

namespace BookBee.Services.AuthService
{
    public interface IAuthService
    {
		Task<ResponseDTO> Login(string username, string password);
        Task<ResponseDTO> Register(RegisterUserDTO registerUserDTO);
		Task<ResponseDTO> ChangePassword(ChangePasswordDTO changePasswordDTO);
        Task<ResponseDTO> ForgotPassword(string email);
		Task<ResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordDTO);


	}
}
