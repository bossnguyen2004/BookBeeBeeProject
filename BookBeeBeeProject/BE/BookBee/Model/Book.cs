using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        public int NumberOfPages { get; set; }
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;
        [Required]
        public string? Language { get; set; }
        [Required]
        public int Count { get; set; } = 0;
        [Required]
        public double Price { get; set; } = 0;
		[StringLength(255)]
		public string? Image { get; set; }
		public int MaxOrder { get; set; }
        [Required]
        public string? Format { get; set; }
        [Required]
        public string? PageSize { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
		public int Status { get; set; } 
		[Required]
        public int? PublisherId { get; set; }
        public virtual Publisher Publisher { get; set; }
        [Required]
        public int? AuthorId { get; set; }
        public virtual Author Author { get; set; }
        [Required]
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public virtual List<Tag> Tags { get; set; } = new List<Tag>();
        public virtual List<Voucher> Vouchers { get; set; } = new List<Voucher>();
        public virtual List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual List<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
