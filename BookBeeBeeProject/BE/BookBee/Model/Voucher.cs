using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? VoucherCode { get; set; }
        [Required]
        public string? VoucherName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public double? DiscountValue { get; set; }
        public int? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public virtual List<Book> Books { get; set; } = new List<Book>();
        public virtual ICollection<VoucherDetail>? VoucherDetails { get; set; }
    }
}
