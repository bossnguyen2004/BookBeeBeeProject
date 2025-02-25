using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.PaymentMethodRepository
{
	public interface IPaymentMethodRepository
	{
		Task<ResponseDTO> CreatePayment(PaymentMethod payment);
		Task<ResponseDTO> DeletePayment(int id);
		Task<ResponseDTO> UpdatePayment(int id, PaymentMethod payment);
		Task<PaymentMethod> GetPaymentById(int id);
		List<PaymentMethod> GetPayments(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<int> GetPaymentCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}

