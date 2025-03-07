using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookBee.Persistences.Repositories.OrderVoucherRepository
{
	public interface IOrderVoucherRepository
	{
		List<OrderVoucher> GetOrderVouchers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<OrderVoucher> GetOrderVoucherById(int id);
		Task<ResponseDTO> UpdateOrderVoucher(int id, OrderVoucher ordervoucher);
		Task<ResponseDTO> DeleteOrderVoucher(int id);
		Task<ResponseDTO> CreateOrderVoucher(OrderVoucher ordervoucher);
		Task<int> GetOrderVoucherCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
        
    }
}
