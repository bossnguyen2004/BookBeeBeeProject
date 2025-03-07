using BookStack.DTO.CartBook;

namespace BookBee.DTO.OrderDetail
{
    public class RequestBillDTO
    {
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string Username { get; set; }
        public string? CodeVoucher { get; set; }
        public int Payment { get; set; }
        public int phiship2 { get; set; }
        public string? MaPTTT { get; set; }
        public int? trangthaithanhtoan { get; set; }
        public string? MaHoaDon { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // Thuộc tính mới để lưu trữ PTTT
        public List<CartBookDTO>? CartItem { get; set; }
    }
    public enum PaymentMethod
    {
        ThanhToanTaiCuaHang = 1,
        ThanhToanKhiNhanHang = 2,
        ChuyenKhoanNganHang = 3,
        VNPay = 4

    }
}
