using BookBee.DTO.Response;
using BookBee.Model;

namespace BookStack.Persistence.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null);
        Task<ResponseDTO> DeleteOrder(int id);
		Task<int> GetOrderCountByUser(int userId);
		Task<int> GetOrderCount();
		Task<bool> IsSaveChanges();
		public int Total { get; }
        Task<List<Order>> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null);
        Task<ResponseDTO> CreateOrder(Order order);
        Task<ResponseDTO> UpdateOrder(int id, Order order);
        Task<Order> GetOrderById(int id);


    }
}
