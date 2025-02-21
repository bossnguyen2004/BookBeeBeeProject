using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string Username { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[32];

        [Required]
        public byte[] PasswordHash { get; set; } = new byte[32];

        [Required]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsVerified { get; set; } = false;

        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;


        public virtual UserProfile UserProfile { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
