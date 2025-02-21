using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class UserProfile
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
        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        public virtual List<Address> Addresses { get; set; } = new List<Address>();

        public virtual List<Order> Orders { get; set; } = new List<Order>();

        public UserProfile()
        {
            Cart = new Cart();
        }
    }
}
