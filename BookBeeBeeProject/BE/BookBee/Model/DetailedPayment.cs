using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class DetailedPayment
    {
        [Key]
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentId { get; set; }
        [Required]
        public double? Price { get; set; }
        public int Status { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual Order? Order { get; set; }
    }
}
