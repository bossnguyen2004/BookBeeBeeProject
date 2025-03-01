using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences;
using EllipticCurve.Utils;
using Microsoft.EntityFrameworkCore;

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




		public async Task<List<Order>> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "")
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

		public async Task<List<Order>> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "")
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

		
	}
}
