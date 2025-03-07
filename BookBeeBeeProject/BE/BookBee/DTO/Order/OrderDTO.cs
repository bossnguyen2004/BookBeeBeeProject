
using BookBee.DTO.Address;
using BookBee.DTO.Employee;
using BookBee.DTO.OrderVoucher;
using BookBee.DTO.User;
using BookBee.DTOs.OrderDetail;
using System.ComponentModel.DataAnnotations;

namespace BookStack.DTOs.Order
{
    public class OrderDTO
    {
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
		public DateTime Create { get; set; }
        public DateTime Update { get; set; }
        public int Count { get; set; } = 0;
        public string? Phuongthucthanhtoan { get; set; }
        public string? CodeVoucher { get; set; }
        public double? GiamGia { get; set; }
        public int AddressId { get; set; }
        public int? OrderVoucherId { get; set; }
        public bool IsPayment { get; set; }
        public int Payment { get; set; }
      
        public int? EmployeeId { get; set; }
        public int UserAccountId { get; set; }
        public object? BillDetail { get; set; }
        public virtual List<int> BookIds { get; set; }
        public List<int> QuantitieCounts { get; set; } = new List<int>();
        public virtual List<OrderDetailDTO> OrderDetails { get; set; }

    }
}
