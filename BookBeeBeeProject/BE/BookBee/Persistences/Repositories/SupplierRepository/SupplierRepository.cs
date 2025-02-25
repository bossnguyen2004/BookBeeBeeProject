using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.SupplierRepository
{
	public class SupplierRepository:ISupplierRepository
	{
		private readonly DataContext _dataContext;
		public int Total { get; set; }

		public SupplierRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}


		public async Task<ResponseDTO> CreateSupplier(Supplier supplier)
		{
			try
			{
				await _dataContext.Suppliers.AddAsync(supplier);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteSupplier(int id)
		{
			var supplier = await _dataContext.Suppliers.FindAsync(id);
			if (supplier == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Suppliers.Remove(supplier);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public List<Supplier> GetSupplier(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.Suppliers.Where(a => !a.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(a => a.Name.ToLower().Contains(key.ToLower()));
			}
			switch (sortBy)
			{
				case "NAME":
					query = query.OrderBy(u => u.Name);
					break;
				default:
					query = query.OrderBy(u => u.IsDeleted).ThenBy(u => u.Id);
					break;
			}
			Total = query.Count();
			if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
			else
				return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
		}

		public async Task<Supplier> GetSupplierById(int id)
		{
			return await _dataContext.Suppliers.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<int> GetSupplierCount()
		{
			return await _dataContext.Suppliers.CountAsync(t => !t.IsDeleted);
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}

		public async Task<ResponseDTO> UpdateSupplier(int id, Supplier supplier)
		{
			var existingSupplier = await _dataContext.Suppliers.FindAsync(id);
			if (existingSupplier == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingSupplier.Name = supplier.Name;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}
	}
}
