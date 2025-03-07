using BookBee.Services.OrderService;
using BookStack.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var res =await _orderService.GetOrderById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "")
        {
            var res = await _orderService.GetOrders(page, pageSize, key, sortBy, status);
            return StatusCode(res.Code, res);
        }
        [HttpGet("History")]
        public async Task<IActionResult> GetHistoryOrders(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res = await _orderService.GetOrderByUser(userId, page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpGet("Self")]
        public async Task<IActionResult> GetSelfHistoryOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res = await _orderService.GetSelfOrders(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO updateOrderDTO)
        {
            var res = await _orderService.UpdateOrder(id, updateOrderDTO);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var res = await _orderService.DeleteOrder(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO createOrderDTO)
        {
            var res = await _orderService.CreateOrder(createOrderDTO);
            return StatusCode(res.Code, res);
        }

        //[HttpPost("Self")]
        //public async Task<IActionResult> SelfCreateOrder(OrderDTO selfCreateOrderDTO)
        //{
        //    var res = await _orderService.SelfCreateOrder(selfCreateOrderDTO);
        //    return StatusCode(res.Code, res);
        //}
    }
}
