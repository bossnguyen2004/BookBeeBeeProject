using AutoMapper;
using BookBee.DTO.Account;
using BookBee.DTO.Employee;
using BookBee.DTO.Response;
using BookBee.DTO.User;
using BookBee.Model;
using BookBee.Persistences;
using BookBee.Persistences.Repositories.AddressRepository;
using BookBee.Persistences.Repositories.CartRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Services.CacheService;
using BookBee.Services.MailService;
using BookBee.Services.TokenService;
using BookBee.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BookBee.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserAccountRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ITokenService _tokenService;
        private readonly ICacheService _cacheService;
        private readonly IMailService _mailService;
        private readonly IAddressRepository _addressRepository;
		private readonly DataContext _dataContext;
		private readonly UserAccessor _userAccessors;
		private const string ForgotPasswordEmailTemplate = $$"""
                                                 <!DOCTYPE html>
                                                 <html lang="vi">
                                                 <head>
                                                     <meta charset="UTF-8">
                                                     <meta http-equiv="X-UA-Compatible" content="IE=edge">
                                                     <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                                     <style>
                                                         body {
                                                             font-family: Arial, sans-serif;
                                                             background-color: #f4f4f4;
                                                             margin: 0;
                                                             padding: 20px;
                                                         }
                                                         .email-container {
                                                             background-color: #ffffff;
                                                             max-width: 600px;
                                                             margin: 0 auto;
                                                             padding: 20px;
                                                             border-radius: 8px;
                                                             box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                                         }
                                                         .header {
                                                             text-align: center;
                                                             padding: 10px 0;
                                                         }
                                                         .header h1 {
                                                             color: #007bff;
                                                             font-size: 24px;
                                                         }
                                                         .logo {
                                                             text-align: center;
                                                             margin-bottom: 10px;
                                                         }
                                                         .logo img {
                                                             width: 120px;
                                                         }
                                                         .content {
                                                             padding: 20px 0;
                                                             text-align: center;
                                                         }
                                                         .content p {
                                                             color: #333;
                                                             line-height: 1.8;
                                                             font-size: 16px;
                                                         }
                                                         .button {
                                                             display: inline-block;
                                                             padding: 12px 24px;
                                                             margin-top: 20px;
                                                             background-color: #007bff;
                                                             color: #ffffff;
                                                             text-decoration: none;
                                                             border-radius: 5px;
                                                             font-weight: bold;
                                                             box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
                                                             transition: background-color 0.3s ease;
                                                         }
                                                         .button:hover {
                                                             background-color: #0056b3;
                                                         }
                                                         .footer {
                                                             text-align: center;
                                                             padding: 20px 0;
                                                             color: #777;
                                                             font-size: 12px;
                                                             border-top: 1px solid #dddddd;
                                                             margin-top: 30px;
                                                         }
                                                         .footer p {
                                                             margin: 0;
                                                         }
                                                     </style>
                                                 </head>
                                                 <body>
                                                     <div class="email-container">
                                                         <div class="logo">
                                                             <img src="https://i.pinimg.com/736x/36/24/e6/3624e650ec342dd00e8bf2b05ead4062.jpg" alt="BookBee Logo">
                                                         </div>
                                                         <div class="header">
                                                             <h1>Đặt lại mật khẩu của bạn</h1>
                                                         </div>
                                                         <div class="content">
                                                             <p>Xin chào,</p>
                                                             <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. Nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
                                                             <a href="{resetLink}" class="button">Đặt lại mật khẩu</a>
                                                             <p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>
                                                         </div>
                                                         <div class="footer">
                                                             <p>&copy; 2025 BookBee. All rights reserved.</p>
                                                         </div>
                                                     </div>
                                                 </body>
                                                 </html>
                                         """;
        private const string RegistrationSuccessEmailTemplate = $$"""
                                                                     <!DOCTYPE html>
                                                                     <html lang="vi">
                                                                     <head>
                                                                         <meta charset="UTF-8">
                                                                         <meta http-equiv="X-UA-Compatible" content="IE=edge">
                                                                         <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                                                         <style>
                                                                             body {
                                                                                 font-family: Arial, sans-serif;
                                                                                 background-color: #f4f4f4;
                                                                                 margin: 0;
                                                                                 padding: 20px;
                                                                             }
                                                                             .email-container {
                                                                                 background-color: #ffffff;
                                                                                 max-width: 600px;
                                                                                 margin: 0 auto;
                                                                                 padding: 20px;
                                                                                 border-radius: 8px;
                                                                                 box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                                                             }
                                                                             .header {
                                                                                 text-align: center;
                                                                                 padding: 10px 0;
                                                                             }
                                                                             .header h1 {
                                                                                 color: #007bff;
                                                                                 font-size: 24px;
                                                                             }
                                                                             .logo {
                                                                                 text-align: center;
                                                                                 margin-bottom: 10px;
                                                                             }
                                                                             .logo img {
                                                                                 width: 120px;
                                                                             }
                                                                             .content {
                                                                                 padding: 20px 0;
                                                                                 text-align: center;
                                                                             }
                                                                             .content p {
                                                                                 color: #333;
                                                                                 line-height: 1.8;
                                                                                 font-size: 16px;
                                                                             }
                                                                             .button {
                                                                                 display: inline-block;
                                                                                 padding: 12px 24px;
                                                                                 margin-top: 20px;
                                                                                 background-color: #007bff;
                                                                                 color: #ffffff;
                                                                                 text-decoration: none;
                                                                                 border-radius: 5px;
                                                                                 font-weight: bold;
                                                                                 box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
                                                                                 transition: background-color 0.3s ease;
                                                                             }
                                                                             .button:hover {
                                                                                 background-color: #218838;
                                                                             }
                                                                             .footer {
                                                                                 text-align: center;
                                                                                 padding: 20px 0;
                                                                                 color: #777;
                                                                                 font-size: 12px;
                                                                                 border-top: 1px solid #dddddd;
                                                                                 margin-top: 30px;
                                                                             }
                                                                             .footer p {
                                                                                 margin: 0;
                                                                             }
                                                                         </style>
                                                                     </head>
                                                                     <body>
                                                                         <div class="email-container">
                                                                             <div class="logo">
                                                                                 <img src="https://i.pinimg.com/736x/36/24/e6/3624e650ec342dd00e8bf2b05ead4062.jpg" alt="BookBee Logo">
                                                                             </div>
                                                                             <div class="header">
                                                                                 <h1>Chào mừng đến với BookBee!</h1>
                                                                             </div>
                                                                             <div class="content">
                                                                                 <p>Xin chào,</p>
                                                                                 <p>Chúc mừng! Bạn đã đăng ký thành công tài khoản tại BookBee.</p>
                                                                                 <p>Bây giờ bạn có thể đăng nhập và bắt đầu khám phá các tính năng của chúng tôi:</p>
                                                                                 <a href="http://localhost:3000/login" class="button">Đăng nhập ngay</a>
                                                                             </div>
                                                                             <div class="footer">
                                                                                 <p>&copy; 2025 BookBee. All rights reserved.</p>
                                                                             </div>
                                                                         </div>
                                                                     </body>
                                                                     </html>
                                                                 """;



        public AuthService(IUserAccountRepository userRepository,
           ICartRepository cartRepository, IMapper mapper,
           ITokenService tokenService, UserAccessor userAccessors,

		   ICacheService cacheService,
		  DataContext dataContext,

		   IMailService mailService, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _cacheService = cacheService;
            _mailService = mailService;
			_userAccessors=userAccessors;
			_addressRepository = addressRepository; _dataContext = dataContext;
		}
        public async Task<ResponseDTO> Login(string username, string password)
        {
            var user =await _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return new ResponseDTO()
                {
                    Code = 401,
                    Message = "UserName không tồn tại"
                };
            }
            if (user.IsDeleted)
            {
                return new ResponseDTO()
                {
                    Code = 403,
                    Message = "Tài khoản đã bị vô hiệu hóa"
                };
            }
            if (!PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return new ResponseDTO()
                {
                    Code = 401,
                    Message = "Mật khẩu không chính xác"
                };
            }

            var token =  _tokenService.GenerateToken(user);

            var data = _mapper.Map<UserAccountDTO>(user);
            if (data == null)
            {
                return new ResponseDTO()
                {
                    Code = 500,
                    Message = "Lỗi ánh xạ dữ liệu"
                };
            }

            data.Token = token;
            return new ResponseDTO()
            {
                Message = "Login thành công",
                Data = data
            };

        }

        public async Task<ResponseDTO> Register(RegisterUserDTO registerUserDTO)
        {
			var user = await _userRepository.GetUserByUsername(registerUserDTO.Username);
			if (user != null)
				return new ResponseDTO()
				{
					Code = 400,
					Message = "Username đã đăng ký"
				};

			var email = _userRepository.GetUserByEmail(registerUserDTO.Email);
			if (email != null)
				return new ResponseDTO()
				{
					Code = 400,
					Message = "Email đã đăng ký"
				};

			if (registerUserDTO.Password != registerUserDTO.CfPassword)
				return new ResponseDTO()
				{
					Code = 400,
					Message = "Password không trùng khớp"
				};

			user = _mapper.Map<UserAccount>(registerUserDTO);
			user.RoleId = 2;

			PasswordHelper.CreatePasswordHash(registerUserDTO.Password, out var passwordHash, out var passwordSalt);
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			_userRepository.CreateUserAccount(user);
			if (await _userRepository.IsSaveChanges())
			{
				return new ResponseDTO()
				{
					Code = 400,
					Message = "Tạo thất bại"
				};
			}

			var address = new Address
			{
				UserAccountId = user.Id,
				Name = string.Empty,
				Phone = string.Empty,
				Street = string.Empty,
				City = string.Empty,
				State = "Mua hàng tại quầy",
				Create = DateTime.Now,
				Update = DateTime.Now,
				IsDeleted = true
			};

			_addressRepository.CreateAddress(address);

			await _mailService.SendEmailAsync(user.Email, "Chào mừng tới BookBee", RegistrationSuccessEmailTemplate);

			return new ResponseDTO()
			{
				Message = "Đăng ký tài khoản thành công"
			};
		}


        public async Task<ResponseDTO> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
			var userId = _userAccessors.GetCurrentUserId();
			if (userId != null)
			{
				var user =await _userRepository.GetUserAccountById((int)userId);
				if (user == null)
				{
					return new ResponseDTO()
					{
						Code = 400,
						Message = "User không tồn tại"
					};
				}
				if (!PasswordHelper.VerifyPasswordHash(changePasswordDTO.OldPassword, user.PasswordHash, user.PasswordSalt))
					return new ResponseDTO()
					{
						Code = 400,
						Message = "Mật khẩu cũ không đúng"
					};
				if (changePasswordDTO.NewPassword != changePasswordDTO.CfPassword)
					return new ResponseDTO()
					{
						Code = 400,
						Message = "Mật khẩu không trùng khớp",
					};
				PasswordHelper.CreatePasswordHash(changePasswordDTO.NewPassword, out var passwordHash, out var passwordSalt);
				user.PasswordHash = passwordHash;
				user.PasswordSalt = passwordSalt;
				_userRepository.UpdateUserAccount(user.Id, user);
				if (await _userRepository.IsSaveChanges())
				{
					return new ResponseDTO()
					{
						Code = 200,
						Message = "Thay đổi mật khẩu thành công"
					};
				}
			}
			return new ResponseDTO()
			{
				Code = 400,
				Message = "Thay đổi mật khẩu thất bại",
			};
		}

        public async Task<ResponseDTO> ForgotPassword(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "User không tồn tại"
                };
            }

            var resetToken = GenerateSecureToken(32);
            _cacheService.Set(resetToken, user.Id, TimeSpan.FromMinutes(5));
            var resetLink = $"http://localhost:3000/reset-password?token={resetToken}";
            var message = ForgotPasswordEmailTemplate.Replace("{resetLink}", resetLink);
            await _mailService.SendEmailAsync(email, "Yêu cầu đổi mật khẩu", message);

            return new ResponseDTO()
            {
                Code = 200,
                Message = "Gửi link quên mật khẩu thành công"
            };
        }

        public async Task<ResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (!_cacheService.Exists(resetPasswordDTO.Token))
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Token không hợp lệ hoặc đã hết hạn"
                };
            }

            var userId = _cacheService.Get<int>(resetPasswordDTO.Token);
            var user = await _userRepository.GetUserAccountById(userId);
            if (user == null)
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "User không tồn tại"
                };
            }

            if (resetPasswordDTO.NewPassword != resetPasswordDTO.CfPassword)
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Mật khẩu không trùng khớp"
                };
            }

            PasswordHelper.CreatePasswordHash(resetPasswordDTO.NewPassword, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
           await _userRepository.UpdateUserAccount(user.Id,user);

            if (await _userRepository.IsSaveChanges())
            {
                _cacheService.Remove(resetPasswordDTO.Token);
                return new ResponseDTO()
                {
                    Code = 200,
                    Message = "Đặt lại mật khẩu thành công"
                };
            }

            return new ResponseDTO()
            {
                Code = 400,
                Message = "Đặt lại mật khẩu thất bại"
            };
        }

        private string GenerateSecureToken(int length)
        {
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "") // To ensure URL safe
                .Replace("/", "")
                .Replace("=", ""); // Removing padding characters
        }

      
    }
}
