using System.ComponentModel.DataAnnotations;

namespace Fe_User.DTO.Account
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Họ không được để trống.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được để trống.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [StringLength(32, ErrorMessage = "Tên đăng nhập không được quá 32 ký tự.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(?:gmail\.com|fpt\.edu\.vn)$",
            ErrorMessage = "Email phải có đuôi @gmail.com hoặc @fpt.edu.vn.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
    ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
