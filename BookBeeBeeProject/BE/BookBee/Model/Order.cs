using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? OrderCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? Description { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShippingAddress { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingFee { get; set; }
        public double TotalAmount { get; set; }
        public int PaymentStatus { get; set; }
        public int DeliveryStatus { get; set; }
		public int? Status { get; set; }
		public string? CancellationReason { get; set; }

		[Required]
        public int? UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        [Required]
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        [Required]
        public int? AddressId { get; set; }
        public virtual Address Address { get; set; }
        [Required]
        public int? OrderVoucherId { get; set; }
        public virtual OrderVoucher OrderVoucher { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public bool IsDeleted { get; set; } = false;
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
    }
}
