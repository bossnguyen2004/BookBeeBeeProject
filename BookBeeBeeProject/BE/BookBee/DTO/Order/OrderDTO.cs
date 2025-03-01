
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

        public virtual UserAccountDTO UserAccount { get; set; }
        public virtual EmployeeDTO Employee { get; set; }
        public virtual AddressDTO Address { get; set; }
		public virtual OrderVoucherDTO OrderVoucher { get; set; }

		public virtual List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
