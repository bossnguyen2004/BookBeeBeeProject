using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.PublisherRepository
{
    public class PublisherRepository : IPublisherRepository
    {
		private readonly DataContext _dataContext;
		public int Total { get; set; }
		public PublisherRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<ResponseDTO> CreatePublisher(Publisher publisher)
		{
			try
			{
				await _dataContext.Publishers.AddAsync(publisher);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

		public async Task<ResponseDTO> DeletePublisher(int id)
		{
			var publisher = await _dataContext.Publishers.FindAsync(id);
			if (publisher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Publishers.Remove(publisher);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public async Task<ResponseDTO> UpdatePublisher(int id, Publisher publisher)
		{
			var existingPublisher = await _dataContext.Publishers.FindAsync(id);
			if (existingPublisher == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingPublisher.Name = publisher.Name;

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

		public async Task<Publisher> GetPublisherById(int id)
		{
			return await _dataContext.Publishers.FirstOrDefaultAsync(a => a.Id == id);
		}

		public List<Publisher> GetPublishers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.Publishers.Where(p => !p.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(p => p.Name.ToLower().Contains(key.ToLower()));
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


		public async Task<bool>  IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}


		public async Task<int> GetPublishersCount()
		{
			return await _dataContext.Publishers.CountAsync();
		}
	}
}
