using System.ComponentModel.DataAnnotations;

namespace BookBee.DTO.PaymentMethod
{
	public class PaymentMethodDTO
	{
		public int Id { get; set; }
		public string? CodePay { get; set; }
		public string? PaymentName { get; set; }
		public string? Description { get; set; }
		public int Status { get; set; }
		public bool IsDeleted { get; set; } = false;
	}
}
