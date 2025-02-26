using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.AddressRepository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DataContext _dataContext;
		public int Total { get; set; }

		public AddressRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

	

		public async Task<ResponseDTO> CreateAddress(Model.Address address)
        {
			try
			{
				await _dataContext.Addresses.AddAsync(address);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}

		}


        public async Task<ResponseDTO> DeleteAddress(int id)
        {
			var address = await _dataContext.Addresses.FindAsync(id);
			if (address == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Addresses.Remove(address);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}


		public async Task<Address> GetAddressById(int id)
		{
			return await _dataContext.Addresses.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<List<Model.Address>> GetAddressByUser(int userId)
        {
			return await _dataContext.Addresses
			 .Include(a => a.UserAccount)
			 .Where(a => a.UserAccount.Id == userId && !a.IsDeleted)
			 .ToListAsync();

		}

        public async Task<int> GetAddressCount()
        {
            return await _dataContext.Addresses.CountAsync(t => !t.IsDeleted);
		}


		public async Task<List<Model.Address>> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
			var query = _dataContext.Addresses.Include(a => a.UserAccount).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(a => (a.Street + ", " + a.City + ", " + a.State + ", " + a.Phone).ToLower().Contains(key.ToLower())); // 🔥 Xóa lặp Phone
			}

			switch (sortBy)
			{
				case "Street":
					query = query.OrderBy(u => u.Street);
					break;
				case "City": // 🔥 Thêm case mới
					query = query.OrderBy(u => u.City);
					break;
				case "State":
					query = query.OrderBy(u => u.State);
					break;
				default:
					query = query.OrderBy(u => u.IsDeleted).ThenBy(u => u.Id);
					break;
			}
			if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
            else
                return query.Where(a => a.IsDeleted == false).Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
        }


        public async Task<bool> IsSaveChanges()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

		public async Task<ResponseDTO> UpdateAddress(int id, Model.Address address)
        {
			var existingAddress = await _dataContext.Addresses.FindAsync(id);
			if (existingAddress == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingAddress.Name = address.Name;
			existingAddress.Street = address.Street;
			existingAddress.City = address.City;
			existingAddress.State = address.State;
			existingAddress.Phone = address.Phone;
			existingAddress.UserAccountId = address.UserAccountId;
			existingAddress.Update = DateTime.Now;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };

		}
    }
}
