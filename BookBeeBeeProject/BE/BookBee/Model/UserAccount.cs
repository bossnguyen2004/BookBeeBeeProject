using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public bool Gender { get; set; } = true;
        public string Phone { get; set; } = "0123456789";
        public DateTime Dob { get; set; } = DateTime.Now;
        public string Avatar { get; set; } = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/59/User-avatar.svg/2048px-User-avatar.svg.png";
        [Required]
        [StringLength(32)]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[32];
        [Required]
        public byte[] PasswordHash { get; set; } = new byte[32];
        public bool IsDeleted { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsVerified { get; set; } = false;
        [Required]
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }

        public int? CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual List<Address> Addresses { get; set; } = new List<Address>();
        public virtual List<Order> Orders { get; set; } = new List<Order>();


        public UserAccount()
        {
            Cart = new Cart();
        }
        public virtual Employee Employee { get; set; }
    }
}
