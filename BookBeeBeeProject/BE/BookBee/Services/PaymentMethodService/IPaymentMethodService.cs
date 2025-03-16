using BookBee.DTO.Author;
using BookBee.DTO.OrderDetail;
using BookBee.DTO.PaymentMethod;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;

namespace BookBee.Services.PaymentMethodService
{
	public interface IPaymentMethodService
	{
        Task<ResponseDTO> GetPaymentMethods(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
        Task<ResponseDTO> GetPaymentMethodById(int id);
        Task<ResponseDTO> UpdatePaymentMethod(int id, PaymentMethodDTO paymentMethodDTO);
        Task<ResponseDTO> DeletePaymentMethod(int id);
        Task<ResponseDTO> CreatePaymentMethod(PaymentMethodDTO paymentMethodDTO);
        List<Model.PaymentMethod> GetAll();
        Task<List<DetailedPayment>> GetAllDetailedPaymentAsync();
        Task<ResponseDTO> CreateToDetailPayment(DetailedPayment detailedPayment);
        ResponseDTO CreateToDetailPayment2(DetailedPayment detailedPayment);
    }
}
