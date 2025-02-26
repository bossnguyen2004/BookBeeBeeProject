using AutoMapper;
using BookBee.DTO.Author;
using BookBee.DTO.PaymentMethod;
using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences.Repositories.PaymentMethodRepository;
using System.ComponentModel.DataAnnotations;

namespace BookBee.Services.PaymentMethodService
{
	public class PaymentMethodService: IPaymentMethodService
	{
		private readonly IPaymentMethodRepository _paymentMethodRepository;
		private readonly IMapper _mapper;
		public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_mapper = mapper;
		}

		public async Task<ResponseDTO> CreatePaymentMethod(PaymentMethodDTO paymentMethodDTO)
		{
			var payment = new PaymentMethod
			{
				CodePay = paymentMethodDTO.CodePay,
				PaymentName = paymentMethodDTO.PaymentName,
				Description = paymentMethodDTO.Description,
				Status = (paymentMethodDTO.Status != 0) ? paymentMethodDTO.Status : 1,
				IsDeleted = false
			};

	    	await _paymentMethodRepository.CreatePayment(payment);
			if (await _paymentMethodRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}

		public async Task<ResponseDTO> DeletePaymentMethod(int id)
		{
			var payment = await _paymentMethodRepository.GetPaymentById(id);
			if (payment == null)
				return new ResponseDTO { Code = 400, Message = "Payment không tồn tại" };
			if (payment.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Payment  đã bị xóa trước đó" };
			payment.IsDeleted = true;
			await _paymentMethodRepository.UpdatePayment(id, payment);
			bool isSaved = await _paymentMethodRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetPaymentMethods(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var payment = _paymentMethodRepository.GetPayments(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<PaymentMethodDTO>>(payment),
				Total = _paymentMethodRepository.Total
			};
		}

		public async Task<ResponseDTO> GetPaymentMethodById(int id)
		{
			var payment = await _paymentMethodRepository.GetPaymentById(id);
			return payment == null || payment.IsDeleted
			? new ResponseDTO { Code = 400, Message = "Payment  không tồn tại hoặc đã bị xóa" }
			: new ResponseDTO { Code = 200, Message = "Lấy Payment  thành công", Data = _mapper.Map<PaymentMethodDTO>(payment) };

		}

		public async Task<ResponseDTO> UpdatePaymentMethod(int id, PaymentMethodDTO paymentMethodDTO)
		{
			var payment = await _paymentMethodRepository.GetPaymentById(id);
			if (payment == null)
				return new ResponseDTO { Code = 400, Message = "payment không tồn tại" };

			payment.Update = DateTime.Now;
			payment.CodePay = paymentMethodDTO.CodePay;
			payment.PaymentName = paymentMethodDTO.PaymentName;
			payment.Description = paymentMethodDTO.Description;
			payment.Status = payment.Status = (paymentMethodDTO.Status != 0) ? paymentMethodDTO.Status : payment.Status;

			await _paymentMethodRepository.UpdatePayment(id, payment);
			bool isSaved = await _paymentMethodRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
	}
}
