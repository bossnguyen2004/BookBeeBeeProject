using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class OrderVoucher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? VoucherCode { get; set; }
        [Required]
        public string? VoucherName { get; set; }
        [Required]
        public int? Discount{ get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
