using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.CategoryRepository
{
    public class TagRepository : ITagRepository
    {
		private readonly DataContext _dataContext;
		public int Total { get; set; }
		public TagRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public List<Tag> GetAllTags(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.Tags.Where(t => !t.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(t => t.Name.ToLower().Contains(key.ToLower()));
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

		public async Task<Tag> GetTagById(int id)
		{
			return await _dataContext.Tags.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<ResponseDTO> CreateTag(Tag tag)
		{
			try
			{
				await _dataContext.Tags.AddAsync(tag);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Thêm thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> UpdateTag(int id, Tag tag)
		{
			var existingTag = await _dataContext.Tags.FindAsync(id);
			if (existingTag == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy thẻ" };

			existingTag.Name = tag.Name;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

		public async Task<ResponseDTO> DeleteTag(int id)
		{
			var tag = await _dataContext.Tags.FindAsync(id);
			if (tag == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy thẻ" };

			tag.IsDeleted = true;
			await _dataContext.SaveChangesAsync();
			return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
		}

		public async Task<int> GetTagsCount()
		{
			return await _dataContext.Tags.CountAsync(t => !t.IsDeleted);
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}
	}
}
