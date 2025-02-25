using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? CodePay{ get; set; }
        [Required]
        public string? PaymentName { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<DetailedPayment>? DetailedPayments { get; set; }
    }
}
