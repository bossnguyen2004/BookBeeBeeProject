using BookBee.DTO.Response;
using BookBee.DTOs.OrderDetail;
using BookBee.Model;
using BookStack.DTOs.Order;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBee.Persistences.Repositories.TaiQuayRepository
{
    public interface ITaiQuayRepository
    {
        public List<Order> GetAll();
        public Task<List<Order>> GetAsync(int? status, int page = 1);
        public Task<OrderDTO> GetBillByInvoiceCode(string invoiceCode);
        Task<IEnumerable<OrderDetailDTO>> GetBillDetailByInvoiceCode(string invoiceCode);
        public Task<ResponseDTO> CancelOrder(int id, string lydo);
        public Task<ResponseDTO> UpdateNgayHoaDonOnline(int idHoaDon, DateTime? NgayThanhToan, DateTime? NgayNhan, DateTime? NgayShip);
        public Task<ResponseDTO> UpdateThanhToan(int idHoaDon, int TrangThaiThanhToan);
        public Task<bool> CheckCustomerExistence(int idId);
        Task<List<Order>> GetByHoaDonStatus(int status);
        Task<ResponseDTO> UpdateHoaDonStatus(int id, int status);
        Task<Order> GetHoaDonByMaHoaDonAsync(string maHoaDon);
        Task<List<Order>> GetCustomerPurchaseHistory(string customerId);
    }
}
