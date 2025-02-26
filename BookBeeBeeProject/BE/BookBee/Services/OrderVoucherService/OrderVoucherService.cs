using AutoMapper;
using BookBee.DTO.OrderVoucher;
using BookBee.DTO.PaymentMethod;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.OrderVoucherRepository;
using BookBee.Persistences.Repositories.VoucherRepository;

namespace BookBee.Services.OrderVoucherService
{
	public class OrderVoucherService : IOrderVoucherService
	{
		private readonly IOrderVoucherRepository _ordervoucherRepository;
		private readonly IMapper _mapper;
		public OrderVoucherService(IOrderVoucherRepository ordervoucherRepository, IMapper mapper)
		{
			_ordervoucherRepository = ordervoucherRepository;
			_mapper = mapper;
		}



		public async Task<ResponseDTO> ChangeOrderVoucherStatus(int id, int status)
		{
			var voucher = await _ordervoucherRepository.GetOrderVoucherById(id);
			if (voucher == null)
				return new ResponseDTO() { Code = 400, Message = "Voucher không tồn tại" };

			voucher.Status = status;
			await _ordervoucherRepository.UpdateOrderVoucher(id, voucher);
			if (await _ordervoucherRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}


		public async Task<ResponseDTO> CreateOrderVoucher(OrderVoucherDTO OrdervoucherDto)
		{

			var ordervoucher = new OrderVoucher
			{
				VoucherCode = OrdervoucherDto.VoucherCode,
				Discount = OrdervoucherDto.Discount,
				DiscountType = OrdervoucherDto.DiscountType,
				MinOrderAmount = OrdervoucherDto.MinOrderAmount,
				MaxDiscountAmount = OrdervoucherDto.MaxDiscountAmount,
				StartDate = OrdervoucherDto.StartDate,
				EndDate = OrdervoucherDto.EndDate,
				Status = OrdervoucherDto.Status ?? 1,
				IsDeleted = false,
				Description = OrdervoucherDto.Description,
				UsageLimit = OrdervoucherDto.UsageLimit,
				UsedCount = OrdervoucherDto.UsedCount
			};

			await _ordervoucherRepository.CreateOrderVoucher(ordervoucher);
			if (await _ordervoucherRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}


		public async Task<ResponseDTO> DeleteOrderVoucher(int id)
		{
			var voucher = await _ordervoucherRepository.GetOrderVoucherById(id);
			if (voucher == null)
				return new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại" };
			if (voucher.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Khuyến  Mại đã bị xóa trước đó" };
			voucher.IsDeleted = true;
			await _ordervoucherRepository.UpdateOrderVoucher(id, voucher);
			bool isSaved = await _ordervoucherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetOrderVoucher(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var ordervoucher =  _ordervoucherRepository.GetOrderVouchers(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<OrderVoucherDTO>>(ordervoucher),
				Total = _ordervoucherRepository.Total
			};
		}

		public async Task<ResponseDTO> GetOrderVoucherById(int id)
		{
			var voucher = await _ordervoucherRepository.GetOrderVoucherById(id);

			return voucher == null || voucher.IsDeleted
			? new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại hoặc đã bị xóa" }
			: new ResponseDTO { Code = 200, Message = "Lấy Khuyến Mại  thành công", Data = _mapper.Map<OrderVoucherDTO>(voucher) };

		}

		public async Task<ResponseDTO> UpdateOrderVoucher(int id, OrderVoucherDTO OrdervoucherDto)
		{

			var ordervoucher = await _ordervoucherRepository.GetOrderVoucherById(id);
			if (ordervoucher == null)
				return new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại" };

			ordervoucher.VoucherCode = OrdervoucherDto.VoucherCode;
			ordervoucher.Discount = OrdervoucherDto.Discount;
			ordervoucher.DiscountType = OrdervoucherDto.DiscountType;
			ordervoucher.MinOrderAmount = OrdervoucherDto.MinOrderAmount;
			ordervoucher.MaxDiscountAmount = OrdervoucherDto.MaxDiscountAmount;
			ordervoucher.StartDate = OrdervoucherDto.StartDate;
			ordervoucher.EndDate = OrdervoucherDto.EndDate;
			ordervoucher.Status = OrdervoucherDto.Status ?? ordervoucher.Status;
			ordervoucher.IsDeleted = OrdervoucherDto.IsDeleted;
			ordervoucher.Description = OrdervoucherDto.Description;
			ordervoucher.UsageLimit = OrdervoucherDto.UsageLimit;
			ordervoucher.UsedCount = OrdervoucherDto.UsedCount;


			await _ordervoucherRepository.UpdateOrderVoucher(id, ordervoucher);
			bool isSaved = await _ordervoucherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
	}
}
