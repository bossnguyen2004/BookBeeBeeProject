using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.VoucherRepository
{
    public class VoucherRepository : IVoucherRepository
    {
		private readonly DataContext _dataContext;
		public int Total { get; set; }
		public VoucherRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<ResponseDTO> CreateVoucher(Voucher voucher)
		{
			try
			{
				await _dataContext.Vouchers.AddAsync(voucher);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteVoucher(int id)
		{
			var voucher = await _dataContext.Vouchers.FindAsync(id);
			if (voucher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Vouchers.Remove(voucher);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public async Task<ResponseDTO> UpdateVoucher(int id, Voucher voucher)
		{
			var existingVoucher = await _dataContext.Vouchers.FindAsync(id);
			if (existingVoucher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingVoucher.VoucherCode = voucher.VoucherCode;
			existingVoucher.VoucherName = voucher.VoucherName;
			existingVoucher.StartDate = voucher.StartDate;
			existingVoucher.EndDate = voucher.EndDate;
			existingVoucher.DiscountValue = voucher.DiscountValue;
			existingVoucher.Status = voucher.Status;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

		public async Task<Voucher> GetVoucherById(int id)
		{
			return await _dataContext.Vouchers.FirstOrDefaultAsync(a => a.Id == id);
		}

		public List<Voucher> GetVouchers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? status = null)
		{
			var query = _dataContext.Vouchers.Where(v => !v.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(v =>
					v.VoucherName.ToLower().Contains(key.ToLower()) ||
					v.VoucherCode.ToLower().Contains(key.ToLower())
				);
			}

			switch (sortBy?.ToUpper())
			{
				case "VOUCHERNAME":
					query = query.OrderBy(v => v.VoucherName);
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

		public async Task<int> GetVoucherCount()
		{
			return  await _dataContext.Vouchers.CountAsync();
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}
	}
}
