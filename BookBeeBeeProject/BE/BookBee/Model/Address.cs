using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Phone { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        [Required]
        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
