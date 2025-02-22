using System.ComponentModel.DataAnnotations;

namespace Fe_User.DTO.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
