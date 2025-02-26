using BookBee.Model;

namespace BookBee.DTO.OrderVoucher
{
	public class OrderVoucherDTO
	{
		public int Id { get; set; }
		public string? VoucherCode { get; set; }
		public double? Discount { get; set; }
		public DiscountType DiscountType { get; set; }
		public double? MinOrderAmount { get; set; }
		public double? MaxDiscountAmount { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int? Status { get; set; }
		public bool IsDeleted { get; set; }
		public string? Description { get; set; }
		public int? UsageLimit { get; set; }
		public int UsedCount { get; set; }
	}
}
