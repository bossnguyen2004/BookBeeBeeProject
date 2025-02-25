using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public class UserAccountRepository : IUserAccountRepository
	{
        private readonly DataContext _dataContext;

        public UserAccountRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int Total { get; set; }
        public async Task<ResponseDTO> CreateUserAccount(UserAccount user)

		{
			try
			{
				await _dataContext.UserAccounts.AddAsync(user);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Thêm thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteUserAccount(int id)
        {
			var user = await _dataContext.UserAccounts.FindAsync(id);
			if (user == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy thẻ" };

			user.IsDeleted = true;
			await _dataContext.SaveChangesAsync();
			return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
		}

		public async Task<UserAccount> GetUserAccountById(int id)
        {
			return await _dataContext.UserAccounts.Include(r => r.Role).FirstOrDefaultAsync(u => u.Id == id);
		}

        public async Task<UserAccount> GetUserByUsername(string username)
        {
            return await _dataContext.UserAccounts.Include(r => r.Role).FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserAccount> GetUserByEmail(string email)
        {
            return await _dataContext.UserAccounts.Include(r => r.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> GetUserAccountCount()
        {
			return await _dataContext.UserAccounts.CountAsync(t => !t.IsDeleted);
		}


	

        public async Task<bool> IsSaveChanges()
        {
            return   await _dataContext.SaveChangesAsync() > 0; ;
        }

        public async Task<ResponseDTO>UpdateUserAccount(int id, UserAccount user)
        {
			var existingUser = await _dataContext.UserAccounts.FindAsync(id);
			if (existingUser == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy user" };

			existingUser.FirstName = user.FirstName;
			existingUser.LastName = user.LastName;
			existingUser.Email = user.Email;
			existingUser.Gender = user.Gender;
			existingUser.Phone = user.Phone;
			existingUser.Dob = user.Dob;
			existingUser.Avatar = user.Avatar;
			existingUser.Username = user.Username;
			existingUser.RoleId = user.RoleId;
			existingUser.IsVerified = user.IsVerified;
			existingUser.Update = DateTime.Now;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };

		}

		public async Task<List<UserAccount>> GetAllUserAccount(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.UserAccounts.Where(t => !t.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(u =>
					(u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(key.ToLower()) ||
					u.Username.ToLower().Contains(key.ToLower()) ||
					u.Email.ToLower().Contains(key.ToLower()) ||
					u.Phone.ToLower().Contains(key.ToLower()));
			}

			switch (sortBy)
			{
				case "NAME":
					query = query.OrderBy(u => u.LastName);
					break;
				default:
					query = query.OrderBy(u => u.IsDeleted).ThenByDescending(u => u.Create);
					break;
			}

			Total = query.Count();

			if (page == null || pageSize == null || sortBy == null)
			{
				return query.ToList();
			}
			else
			{
				return query.Where(r => r.Username != "guest")
							.Skip((page.Value - 1) * pageSize.Value)
							.Take(pageSize.Value)
							.ToList();
			}
		}


	}
}
