using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences;
using EllipticCurve.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Drawing;
using static BookBee.Model.TrangThai;

namespace BookStack.Persistence.Repositories.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _dataContext;
        public OrderRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public int Total { get; set; }


        public async Task<ResponseDTO> CreateOrder(Order order)
        {
			try
			{
				await _dataContext.Orders.AddAsync(order);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteOrder(int id)
        {
			var order = await _dataContext.Orders.FindAsync(id);
			if (order == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Orders.Remove(order);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}


		public async Task<Order> GetOrderById(int id)
        {
            return await _dataContext.Orders
                .Include(u => u.UserAccount).Include(v => v.OrderVoucher).Include(e => e.Employee)
				.Include(a => a.Address).Include(or => or.OrderDetails)
				.ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
        }


		public async Task<List<Order>> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null)
        {
			if (userId <= 0) return new List<Order>();
			var query = _dataContext.Orders
				.Include(u => u.UserAccount)
				.Include(v => v.OrderVoucher)
				.Include(a => a.Address)
				.Include(e => e.Employee)
				.Include(o => o.OrderDetails)
				.ThenInclude(s => s.Book)
				.AsSplitQuery()
				.Where(o => o.UserAccountId == userId)
				.AsQueryable();

			if (!string.IsNullOrEmpty(status) && int.TryParse(status, out int statusValue))
			{
				query = query.Where(o => o.Status == statusValue);
			}
            if (orderType.HasValue)
            {
                query = query.Where(o => (int)o.OrderType == orderType.Value);
            }
            if (!string.IsNullOrEmpty(key))
			{
				if (int.TryParse(key, out int keyValue))
				{
					query = query.Where(o => o.Id == keyValue || o.OrderDetails.Any(b => b.Book.Title.Contains(key)));
				}
				else
				{
					query = query.Where(o => o.UserAccount.Username.Contains(key) || o.OrderDetails.Any(b => b.Book.Title.Contains(key)));
				}
			}

			switch (sortBy)
			{
				case "CREATE":
					query = query.OrderByDescending(u => u.Create.Date).ThenByDescending(u => u.Create.Hour)
								 .ThenByDescending(u => u.Create.Minute).ThenByDescending(u => u.Id);break;
				case "CREATE_DESC":
					query = query.OrderBy(u => u.Create.Date).ThenBy(u => u.Create.Hour)
								 .ThenBy(u => u.Create.Minute).ThenBy(u => u.Id);break;
				default:
					query = query.OrderBy(u => u.IsDeleted).ThenByDescending(u => u.Id);break;
			}

			if (page == null || pageSize == null || sortBy == null) { return await query.ToListAsync(); }
            else
                return await query.Where(o => o.IsDeleted == false).Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
        }


		public async Task<int> GetOrderCountByUser(int userId)
		{
			return await  _dataContext.Orders.CountAsync(o => o.UserAccountId == userId && o.IsDeleted == false);
		}

		public async Task<List<Order>> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null)
        {
            if (!_dataContext.Orders.Any()) return Enumerable.Empty<Order>().ToList();


            var query = _dataContext.Orders.Where(r => r.IsDeleted == false)
				.Include(u => u.UserAccount).Include(v => v.OrderVoucher)
				.Include(a => a.Address).Include(e => e.Employee)
				.Include(o => o.OrderDetails).ThenInclude(s => s.Book).AsQueryable();

			if (!string.IsNullOrEmpty(status) && int.TryParse(status, out int statusValue))
			{
				query = query.Where(o => o.Status == statusValue);
            }
            if (orderType.HasValue)
            {
                query = query.Where(o => (int)o.OrderType == orderType.Value);
            }

            if (!string.IsNullOrEmpty(key))
            {
                if (int.TryParse(key, out int id))
                {
                    query = query.Where(au => au.Id == id);
                }
            }

            switch (sortBy)
            {
                case "CREATE":
                    query = query.OrderBy(u => u.Create).ThenBy(u => u.Id);
                    break;
                case "CREATE_DESC":
                    query = query.OrderByDescending(u => u.Create).ThenBy(u => u.Id);
                    break;
                default:
                    query = query.OrderBy(u => u.IsDeleted).ThenBy(u => u.Id);
                    break;
            }
            
            Total = query.Count();

			if (page == null || pageSize == null || sortBy == null) { return await query.ToListAsync(); }

			return await  query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}

		public async Task<int> GetOrderCount()
		{
			return await _dataContext.Orders.CountAsync(t => !t.IsDeleted);
		}

		public async Task<ResponseDTO> UpdateOrder(int id, Order order)
		{
			var existingOrder = await _dataContext.Orders.FindAsync(id);
			if (existingOrder == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingOrder.OrderCode = order.OrderCode;
			existingOrder.CreatedDate = order.CreatedDate;
			existingOrder.PaymentDate = order.PaymentDate;
			existingOrder.ShippingDate = order.ShippingDate;
			existingOrder.ReceivedDate = order.ReceivedDate;
			existingOrder.Description = order.Description;
			existingOrder.CustomerName = order.CustomerName;
			existingOrder.PhoneNumber = order.PhoneNumber;
			existingOrder.ShippingAddress = order.ShippingAddress;
			existingOrder.DiscountAmount = order.DiscountAmount;
			existingOrder.ShippingFee = order.ShippingFee;
			existingOrder.TotalAmount = order.TotalAmount;
			existingOrder.PaymentStatus = order.PaymentStatus;
			existingOrder.DeliveryStatus = order.DeliveryStatus;
			existingOrder.Status = order.Status;
			existingOrder.CancellationReason = order.CancellationReason;
			existingOrder.UserAccountId = order.UserAccountId;
			existingOrder.EmployeeId = order.EmployeeId;
			existingOrder.AddressId = order.AddressId;
			existingOrder.OrderVoucherId = order.OrderVoucherId;
			existingOrder.IsDeleted = order.IsDeleted;
			existingOrder.Update = DateTime.Now;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
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
                            sanPhamChiTiet.Count += chiTiet.Quantity;
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
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = $"Lỗi khi cập nhật ngày hóa đơn: {ex.Message}" };
            }
        }

        public async Task<ResponseDTO> UpdateThanhToan(int id, int TrangThaiThanhToan)
        {
            try
            {
                var hoaDon = await _dataContext.Orders.FindAsync(id);

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

        public async Task<bool> CheckCustomerExistence(int customerId)
        {
            var customer = await _dataContext.UserAccounts.FindAsync(customerId);
            return customer != null;
        }

        public async Task<List<Order>> GetByHoaDonStatus(int HoaDonStatus)
        {
            var hoaDons = await _dataContext.Orders
               .Where(hd => hd.DeliveryStatus == HoaDonStatus)
               .ToListAsync();

            return hoaDons;
        }


        public async Task<ResponseDTO> UpdateHoaDonStatus(int id, int HoaDonStatus)
        {
            var hoaDon = await _dataContext.Orders.FirstOrDefaultAsync(hd => hd.Id == id);
            if (hoaDon != null)
            {
                var chiTietHoaDon = await _dataContext.OrderDetails
                    .Include(ct => ct.Book)
                    .Where(ct => ct.OrderId == hoaDon.Id)
                    .ToListAsync();

                if ((DeliveryStatus)HoaDonStatus == DeliveryStatus.DangGiaoHang)
                {
                    if (hoaDon != null)
                    {
                        foreach (var chiTiet in chiTietHoaDon) 
                        {
                            var sanPhamChiTiet = chiTiet.Book;
                            if (sanPhamChiTiet != null)
                            {
                                if (sanPhamChiTiet.Count >= chiTiet.Quantity)
                                {
                                    sanPhamChiTiet.Count -= chiTiet.Quantity;
                                }
                                else
                                {
                                    return new ResponseDTO { IsSuccess = false, Message = "Số lượng tồn không đủ", Code = 400 };
                                }
                            }
                        }
                    }
                }
                else if (HoaDonStatus == (int)DeliveryStatus.DaGiaoHang)
                {

                    if (hoaDon != null)
                    {
                        foreach (var chiTiet in chiTietHoaDon)
                        {
                            var sanPhamChiTiet = chiTiet.Book;
                            if (sanPhamChiTiet != null)
                            {
                                sanPhamChiTiet.Count += chiTiet.Quantity;
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
              (DeliveryStatus)HoaDonStatus == DeliveryStatus.DaGiaoHang)
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
                     (DeliveryStatus)HoaDonStatus == DeliveryStatus.DaGiaoHang)
            {

                hoaDon.ShippingDate = DateTime.Now;
                hoaDon.ReceivedDate = DateTime.Now;
            }
            hoaDon.DeliveryStatus = HoaDonStatus;
            await _dataContext.SaveChangesAsync();
            return new ResponseDTO { IsSuccess = true, Message = "Cập nhật trạng thái hóa đơn thành công" };
        }

        public async Task<Order> GetHoaDonByMaHoaDonAsync(string maHoaDon)
        {
            return await _dataContext.Orders.FirstOrDefaultAsync(h => h.OrderCode == maHoaDon);
        }

        public async Task<List<Order>> GetCustomerPurchaseHistory(string customerId)
        {
            var user = _dataContext.UserAccounts.FirstOrDefault(x => x.FirstName == customerId).Id;
            return _dataContext.Orders.Where(hd => hd.UserAccountId == user).ToList();
        }


    }
}
