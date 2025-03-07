using BookBee.DTO.Response;
using BookStack.DTO.CartBook;
using BookStack.DTOs.Order;

namespace BookBee.Services.OrderService
{
    public interface IOrderService
    {
        Task<ResponseDTO> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null);
        Task<ResponseDTO> GetOrderById(int id);
        Task<ResponseDTO> UpdateOrder(int id, OrderDTO updateOrderDTO);
        Task<ResponseDTO> DeleteOrder(int id);
        Task<ResponseDTO> CreateOrder(OrderDTO createOrderDTO);
        //Task<ResponseDTO> SelfCreateOrder(OrderDTO selfCreateOrderDTO);
        Task<ResponseDTO> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? orderType = null);
        Task<ResponseDTO> GetSelfOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");


    }
}
