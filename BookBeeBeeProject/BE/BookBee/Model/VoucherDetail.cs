using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class VoucherDetail
    {
        [Key]
        public int Id { get; set; }
        public int? VoucherId { get; set; }
        public int? BookId { get; set; }
        public int Status { get; set; }

        public virtual Voucher? Vouchers { get; set; }
        public virtual Book? Books
        {
            get; set;
        }
    }
}
