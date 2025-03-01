using BookBee.DTO.Response;
using BookBee.Model;

namespace BookStack.Persistence.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "");
		Task<List<Order>> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "");
        Task<Order> GetOrderById(int id);
        Task<ResponseDTO> UpdateOrder(int id,Order order);
        Task<ResponseDTO> DeleteOrder(int id);
		Task<ResponseDTO> CreateOrder(Order order);
		Task<int> GetOrderCountByUser(int userId);
		Task<int> GetOrderCount();
		Task<bool> IsSaveChanges();
		public int Total { get; }
    }
}
