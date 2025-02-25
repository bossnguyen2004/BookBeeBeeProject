using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.AuthorRepository
{
    public class AuthorRepository : IAuthorRepository
    {
		private readonly DataContext _dataContext;
		public int Total { get; set; }

		public AuthorRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<ResponseDTO> CreateAuthor(Author author)
		{
			try
			{
				await _dataContext.Authors.AddAsync(author);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteAuthor(int id)
		{
			var author = await _dataContext.Authors.FindAsync(id);
			if (author == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Authors.Remove(author);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public async Task<ResponseDTO> UpdateAuthor(int id, Author author)
		{
			var existingAuthor = await _dataContext.Authors.FindAsync(id);
			if (existingAuthor == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingAuthor.Name = author.Name;
			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

		public async Task<Author> GetAuthorById(int id)
		{
			return await  _dataContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
		}

		public List<Author> GetAuthors(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.Authors.Where(a => !a.IsDeleted).AsQueryable();

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

		public async Task<int> GetAuthorCount()
		{
			return await _dataContext.Authors.CountAsync(t => !t.IsDeleted);
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}
	}
}

