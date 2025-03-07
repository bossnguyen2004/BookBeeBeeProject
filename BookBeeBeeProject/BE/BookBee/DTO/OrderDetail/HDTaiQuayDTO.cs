using BookStack.DTO.CartBook;

namespace BookBee.DTO.OrderDetail
{
    public class HDTaiQuayDTO
    {
        public int IdNguoiDung { get; set; }
        public int? IdNhanVien { get; set; }
        public DateTime? NgayTao { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public double TongTienHang { get; set; }
        public int? VoucherId { get; set; }
        public string? MaVoucher { get; set; }
        public double? TongThanhToan { get; set; }
        public List<CartBookDTO>? CartItem { get; set; }

    }
}
