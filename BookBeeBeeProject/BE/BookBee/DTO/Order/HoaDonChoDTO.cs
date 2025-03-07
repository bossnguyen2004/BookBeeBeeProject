using BookBee.DTOs.OrderDetail;

namespace BookBee.DTO.Order
{
    public class HoaDonChoDTO
    {
        public int Id { get; set; }
        public int? UserAccountId { get; set; }
        public int? EmployeeId { get; set; }
        public int? IdVoucher { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public string? OrderCode { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? DeliveryStatus { get; set; }
        public int? PaymentStatus { get; set; }
        public int? TienGiam { get; set; }
        public int? GiamGia { get; set; }
        public int? TotalAmount { get; set; }
        public string? Phuongthucthanhtoan { get; set; }
        public string? NguoiTao { get; set; }
        public List<OrderDetailDTO> OrderDetailDTOs { get; set; }
    }
}
