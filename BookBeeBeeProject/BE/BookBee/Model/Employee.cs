using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        public DateTime BirthYear { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Hometown { get; set; }
		public int? Status { get; set; }
		public bool IsDeleted { get; set; } = false;

		[Required]
        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
