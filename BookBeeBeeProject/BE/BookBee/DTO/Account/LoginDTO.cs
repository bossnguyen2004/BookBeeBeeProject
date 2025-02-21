using System.ComponentModel.DataAnnotations;

namespace BookBee.DTO.Account
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
