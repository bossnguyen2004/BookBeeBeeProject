using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookBee.Persistences.Repositories.OrderVoucherRepository
{
	public class OrderVoucherRepository : IOrderVoucherRepository
	{

		private readonly DataContext _dataContext;
		public int Total { get; set; }
		public OrderVoucherRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
       
        public async Task<ResponseDTO> CreateOrderVoucher(OrderVoucher ordervoucher)
		{
			try
			{
				await _dataContext.OrderVouchers.AddAsync(ordervoucher);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}


		public async Task<ResponseDTO> DeleteOrderVoucher(int id)
		{
			var ordervoucher = await _dataContext.OrderVouchers.FindAsync(id);
			if (ordervoucher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.OrderVouchers.Remove(ordervoucher);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public async Task<OrderVoucher> GetOrderVoucherById(int id)
		{
			return await _dataContext.OrderVouchers.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<int> GetOrderVoucherCount()
		{
			return await _dataContext.OrderVouchers.CountAsync();
		}

		public List<OrderVoucher> GetOrderVouchers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.OrderVouchers.Where(v => !v.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				key = key.ToLower();
				query = query.Where(v => v.VoucherCode.ToLower().Contains(key) ||
										 v.Description.ToLower().Contains(key));
			}

			switch (sortBy?.ToUpper())
			{
				case "VOUCHERNAME":
					query = query.OrderBy(v => v.VoucherCode);
					break;
				default:
					query = query.OrderBy(v => v.IsDeleted).ThenBy(v => v.Id);
					break;
			}
			Total = query.Count();
			if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
			else
				return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}

		public async Task<ResponseDTO> UpdateOrderVoucher(int id, OrderVoucher ordervoucher)
		{
			var existingOrderVoucher = await _dataContext.OrderVouchers.FindAsync(id);
			if (existingOrderVoucher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingOrderVoucher.VoucherCode = ordervoucher.VoucherCode;
			existingOrderVoucher.Discount = ordervoucher.Discount;
			existingOrderVoucher.DiscountType = ordervoucher.DiscountType;
			existingOrderVoucher.MinOrderAmount = ordervoucher.MinOrderAmount;
			existingOrderVoucher.MaxDiscountAmount = ordervoucher.MaxDiscountAmount;
			existingOrderVoucher.StartDate = ordervoucher.StartDate;
			existingOrderVoucher.EndDate = ordervoucher.EndDate;
			existingOrderVoucher.Status = ordervoucher.Status;
			existingOrderVoucher.UsageLimit = ordervoucher.UsageLimit;
			existingOrderVoucher.UsedCount = ordervoucher.UsedCount;
			existingOrderVoucher.Description = ordervoucher.Description;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

       
    }
}
