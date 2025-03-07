using BookBee.DTO.Order;
using BookBee.DTO.OrderDetail;
using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Services.TaiQuayServices
{
    public interface ITaiQuayServices
    {
        public Task<ResponseDTO> CreateBill(RequestBillDTO requestBill);
        public Task<ResponseDTO> PGetBillByInvoiceCode(string invoiceCode);
        public Task<ResponseDTO> CreateHoaDonTaiQuay(RequestBillDTO requestBill);
        public List<HoaDonChoDTO> GetAllHDTaiQuay();
        public ResponseDTO TaoHoaDonTaiQuay(HDTaiQuayDTO requestHDTaiQuay);
        public Task<ResponseDTO> AddBillDetail(string mahoadon, string codeProductDetail, int? soluong);
        public ResponseDTO CapNhatSoLuongHoaDonChiTietTaiQuay(string maHoaDon, string maSPCT, int soLuong);
        public Task<ResponseDTO> TruQuantityBillDetail(int idBillDetail);
        public Task<ResponseDTO> CongQuantityBillDetail(int idBillDetail);
        public bool ThanhToan(Order _hoaDon);
        public string XoaSanPhamKhoiHoaDon(string maHoaDon, string maSPCT);
        public Task<ResponseDTO> HuyHoaDonAsync(string maHoaDon, string lyDoHuy);
    }
}
