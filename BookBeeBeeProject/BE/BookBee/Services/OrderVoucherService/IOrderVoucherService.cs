using BookBee.DTO.OrderVoucher;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;

namespace BookBee.Services.OrderVoucherService
{
	public interface IOrderVoucherService
	{
		Task<ResponseDTO> GetOrderVoucher(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetOrderVoucherById(int id);
		Task<ResponseDTO> UpdateOrderVoucher(int id, OrderVoucherDTO OrdervoucherDto);
		Task<ResponseDTO> DeleteOrderVoucher(int id);
		Task<ResponseDTO> CreateOrderVoucher(OrderVoucherDTO OrdervoucherDto);
		Task<ResponseDTO> ChangeOrderVoucherStatus(int id, int status);
	}
}
