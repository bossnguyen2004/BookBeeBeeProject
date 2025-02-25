using BookBee.DTO.Author;
using BookBee.DTO.PaymentMethod;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;

namespace BookBee.Services.PaymentMethodService
{
	public interface IPaymentMethodService
	{
		Task<ResponseDTO> GetPaymentMethods(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetPaymentMethodById(int id);
		Task<ResponseDTO> UpdatePaymentMethod(int id, PaymentMethodDTO paymentMethodDTO);
		Task<ResponseDTO> DeletePaymentMethod(int id);
		Task<ResponseDTO> CreatePaymentMethod(PaymentMethodDTO paymentMethodDTO);
	}
}
