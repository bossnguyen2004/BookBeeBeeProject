using BookBee.DTO.Response;
using BookBee.DTOs.OrderDetail;
using BookBee.Model;
using BookStack.DTOs.Order;
using System.Data.Entity;
using System.Drawing;
using static BookBee.Model.TrangThai;

namespace BookBee.Persistences.Repositories.TaiQuayRepository
{
    public class TaiQuayRepository : ITaiQuayRepository
    {
        private readonly DataContext _dataContext;
        public TaiQuayRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ResponseDTO> CancelOrder(int id, string lydo)
        {
            try
            {
                var hoaDon = await _dataContext.Orders.FindAsync(id);
                if (hoaDon == null)
                {
                    return new ResponseDTO { Code = 400, Message = "Không tìm thấy hóa đơn." };
                }
                if ((DeliveryStatus)hoaDon.DeliveryStatus == DeliveryStatus.DangGiaoHang)

                {
                    return new ResponseDTO { Code = 500, Message = "Không thể hủy đơn hàng khi đang giao hàng ." };
                }
                else if ((DeliveryStatus)hoaDon.DeliveryStatus == DeliveryStatus.DaGiaoHang)
                {
                    return new ResponseDTO { Code = 500, Message = "Không thể hủy đơn hàng khi đã giao hàng." };
                }
                else if ((DeliveryStatus)hoaDon.DeliveryStatus == DeliveryStatus.DaHuy)
                {
                    return new ResponseDTO { Code = 500, Message = "Không thể hủy đơn đã hủy." };
                }

                var chiTietHoaDon = await _dataContext.OrderDetails
                    .Include(ct => ct.Book)
                    .Where(ct => ct.OrderId == hoaDon.Id)
                    .ToListAsync();
                if (chiTietHoaDon != null && chiTietHoaDon.Any())
                {
                    foreach (var chiTiet in chiTietHoaDon)
                    {
                        var sanPhamChiTiet = chiTiet.Book;
                        if (sanPhamChiTiet != null)
                        {
                            // Hoàn lại số lượng tồn
                            sanPhamChiTiet.StockQuantity += chiTiet.Quantity;
                        }
                    }
                }
                hoaDon.PaymentDate = DateTime.Now;
                hoaDon.DeliveryStatus = (int)DeliveryStatus.DaHuy;
                hoaDon.CancellationReason = lydo;

                await _dataContext.SaveChangesAsync();

                return new ResponseDTO { Code = 200, Message = "Hóa đơn đã được hủy thành công." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Code = 500, Message = "Đã xảy ra lỗi khi hủy đơn hàng: " + ex.Message };
            }
        }

        public async  Task<bool> CheckCustomerExistence(int id)
        {
            var customer = await _dataContext.UserAccounts.FindAsync(id);
            return customer != null;
        }

        public async Task<ResponseDTO> UpdateThanhToan(int idHoaDon, int TrangThaiThanhToan)
        {
            try
            {
                var hoaDon = await _dataContext.Orders.FindAsync(idHoaDon);

                if (hoaDon == null)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 400, Message = "Hóa đơn không tồn tại" };
                }

                hoaDon.PaymentStatus = TrangThaiThanhToan;
                _dataContext.Update(hoaDon);
                await _dataContext.SaveChangesAsync();

                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Cập nhật trạng thái thanh toán thành công" };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ResponseDTO> UpdateHoaDonStatus(int id, int status)
        {
            var hoaDon = await _dataContext.Orders.FirstOrDefaultAsync(hd => hd.Id == id);
            if (hoaDon != null)
            {
                var chiTietHoaDon = await _dataContext.OrderDetails
                    .Include(ct => ct.Book)
                    .Where(ct => ct.OrderId == hoaDon.Id)
                    .ToListAsync();

                if ((DeliveryStatus)status == DeliveryStatus.DangGiaoHang)
                {
                    if (hoaDon != null)
                    {
                        foreach (var chiTiet in chiTietHoaDon)
                        {
                            var sanPhamChiTiet = chiTiet.Book;
                            if (sanPhamChiTiet != null)
                            {
                                if (sanPhamChiTiet.StockQuantity >= chiTiet.Quantity)
                                {
                                    sanPhamChiTiet.StockQuantity -= chiTiet.Quantity;
                                }
                                else
                                {
                                    return new ResponseDTO { IsSuccess = false, Message = "Số lượng tồn không đủ", Code = 400 };
                                }
                            }
                        }
                    }
                }

                else if (status == (int)DeliveryStatus.DaGiaoHang)
                {


                    if (hoaDon != null)
                    {
                        foreach (var chiTiet in chiTietHoaDon)
                        {
                            var sanPhamChiTiet = chiTiet.Book;
                            if (sanPhamChiTiet != null)
                            {
                                sanPhamChiTiet.SoldQuantity += chiTiet.Quantity;
                                _dataContext.Books.Update(sanPhamChiTiet);
                            }
                        }

                        await _dataContext.SaveChangesAsync();
                    }
                }
            }
            if (hoaDon == null)
                return new ResponseDTO { Message = "Hóa đơn không tồn tại", Code = 404 };

            if ((PaymentStatus)hoaDon.PaymentStatus == PaymentStatus.Chuathanhtoan &&
                (DeliveryStatus)status == DeliveryStatus.DaGiaoHang)
            {
                if ((PaymentStatus)hoaDon.PaymentStatus == PaymentStatus.Chuathanhtoan)
                {
                    hoaDon.PaymentDate = DateTime.Now;
                    hoaDon.ShippingDate = DateTime.Now;
                    hoaDon.ReceivedDate = DateTime.Now;
                }
                hoaDon.PaymentStatus = (int)PaymentStatus.Dathanhtoan;
            }
            else if ((PaymentStatus)hoaDon.PaymentStatus == PaymentStatus.Dathanhtoan &&
                     (DeliveryStatus)status == DeliveryStatus.DaGiaoHang)
            {

                hoaDon.ShippingDate = DateTime.Now;
                hoaDon.ReceivedDate = DateTime.Now;
            }
            hoaDon.DeliveryStatus = status;
            await _dataContext.SaveChangesAsync();
            return new ResponseDTO { IsSuccess = true, Message = "Cập nhật trạng thái hóa đơn thành công" };
        }

        public async Task<Order> GetHoaDonByMaHoaDonAsync(string maHoaDon)
        {
            return await _dataContext.Orders.FirstOrDefaultAsync(h => h.OrderCode == maHoaDon);
        }

        public async Task<List<Order>> GetCustomerPurchaseHistory(string customerId)
        {
            var user = _dataContext.UserAccounts.FirstOrDefault(x => x.Username == customerId).Id;
            return _dataContext.Orders.Where(hd => hd.UserAccountId == user).ToList();
        }

        public async Task<List<Order>> GetByHoaDonStatus(int status)
        {
            var hoaDons = await _dataContext.Orders
                .Where(hd => hd.DeliveryStatus == status)
                .ToListAsync();

            return hoaDons;
        }

        public async Task<ResponseDTO> UpdateNgayHoaDonOnline(int idHoaDon, DateTime? NgayThanhToan, DateTime? NgayNhan, DateTime? NgayShip)
        {
            try
            {
                var hoadon = await _dataContext.Orders.FindAsync(idHoaDon);
                if (hoadon == null)
                    return new ResponseDTO { IsSuccess = false, Code = 404, Message = "Không tìm thấy hóa đơn." };

                if (hoadon.PaymentStatus == (int)PaymentStatus.Dathanhtoan) 
                {

                    hoadon.ReceivedDate = NgayNhan ?? hoadon.ReceivedDate;
                    hoadon.ShippingDate = NgayShip ?? hoadon.ShippingDate;
                }
                else 
                {

                    hoadon.PaymentDate = NgayThanhToan ?? hoadon.PaymentDate;
                    hoadon.ReceivedDate = NgayNhan ?? hoadon.ReceivedDate;
                    hoadon.ShippingDate = NgayShip ?? hoadon.ShippingDate;
                }

                await _dataContext.SaveChangesAsync();
                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Cập nhật ngày hóa đơn thành công." };
            }
            catch (Exception ex)
            {
                // Xử lý các trường hợp ngoại lệ nếu cần
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = $"Lỗi khi cập nhật ngày hóa đơn: {ex.Message}" };
            }
        }

        public async Task<IEnumerable<OrderDetailDTO>> GetBillDetailByInvoiceCode(string invoiceCode)
        {
            try
            {
                var billDetails = (
                    from x in _dataContext.Orders.AsQueryable().AsNoTracking().Where(a => a.OrderCode == invoiceCode)
                    join y in _dataContext.OrderDetails.AsQueryable().AsNoTracking() on x.Id equals y.OrderId
                    join z in _dataContext.Books.AsQueryable().AsNoTracking() on y.BookId equals z.Id

                    select new OrderDetailDTO
                    {
                        OrderId = x.Id,
                        Id = y.Id,
                        BookId = y.BookId,
                        CodeBook = z.CodeBook,
                        Quantity = y.Quantity,
                        Price = (float)z.GiaThucTe,
                        PriceBan = (float)y.Price,
                    }).AsEnumerable();
                return billDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OrderDTO> GetBillByInvoiceCode(string invoiceCode)
        {
            var query = from bill in _dataContext.Orders
                        where bill.OrderCode == invoiceCode
                        join v in _dataContext.OrderVouchers on bill.OrderVoucherId equals v.Id into voucherGroup
                        from voucher in voucherGroup.DefaultIfEmpty()
                        join ttct in _dataContext.DetailedPayments on bill.Id equals ttct.OrderId into ttctGroup
                        from thanhtoanchitiet in ttctGroup.DefaultIfEmpty()
                        join pttt in _dataContext.PaymentMethods on thanhtoanchitiet.PaymentId equals pttt.Id into ptttGroup
                        from phuongthucthanhtoan in ptttGroup.DefaultIfEmpty()
                        select new OrderDTO
                        {
                            Id = bill.Id,
                            OrderCode = bill.OrderCode,
                            PhoneNumber = bill.PhoneNumber,
                            CustomerName = bill.CustomerName,
                            ShippingAddress = bill.ShippingAddress,
                            ShippingFee = (int)bill.ShippingFee,
                            DeliveryStatus = bill.DeliveryStatus,
                            PaymentStatus = bill.PaymentStatus,
                            CreatedDate = bill.CreatedDate,
                            PaymentDate = bill.PaymentDate,
                            DiscountAmount = (int)bill.DiscountAmount,
                            TotalAmount = (int)bill.TotalAmount,
                            GiamGia = voucher != null
                               ? (voucher.DiscountType == DiscountType.Percentage
                                   ? Math.Min(bill.TotalAmount * voucher.Discount.Value / 100, voucher.MaxDiscountAmount ?? double.MaxValue)
                                   : voucher.Discount.Value) : 0,
                            CodeVoucher = voucher != null ? voucher.VoucherCode : null,
                            UserAccountId = bill.UserAccountId.Value,

                            Phuongthucthanhtoan = phuongthucthanhtoan.PaymentName

                        };

            return await query.FirstOrDefaultAsync();
        }


        public async Task<List<Order>> GetAsync(int? status, int page = 1)
        {
            var list = _dataContext.Orders.AsQueryable();
            if (status.HasValue)
            {
                list = list.Where(x => x.PaymentStatus == status && x.DeliveryStatus == status);
            }
            var result = list.Select(x => new Order
            {
                Id = x.Id,
                OrderCode = x.OrderCode,
                CreatedDate = x.CreatedDate,
                PaymentDate = x.PaymentDate,
                ShippingDate = x.ShippingDate,
                ReceivedDate = x.ReceivedDate,
                Description = x.Description,
                DiscountAmount = x.DiscountAmount,
                ShippingFee = x.ShippingFee,
                TotalAmount = x.TotalAmount,
                PaymentStatus = x.PaymentStatus,
                DeliveryStatus = x.DeliveryStatus,
                CancellationReason = x.CancellationReason,
                Status= x.Status,
                
            });


            return result.ToList();

        }

        public List<Order> GetAll()
        {
            return _dataContext.Orders.ToList();
        }
    }
}
