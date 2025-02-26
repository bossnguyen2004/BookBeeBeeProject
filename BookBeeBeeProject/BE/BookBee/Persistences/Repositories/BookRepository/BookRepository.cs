using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences;
using Microsoft.EntityFrameworkCore;

namespace BookStack.Persistence.Repositories.BookRepository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _dataContext;

        public int Total { get; private set; }

        public BookRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

		public async Task<List<Book>> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0, int? imgId = 0, bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? supplierId = null)
		{
			var query = _dataContext.Books.Include(a => a.Author).Include(p => p.Publisher)
				.Include(s => s.Supplier).Include(t => t.Tags).Include(v => v.Vouchers)
				.Include(i => i.Imgs).Include(o => o.OrderDetails).ThenInclude(od => od.Order)
				.AsSplitQuery().AsQueryable();

			if (tagId > 0){query = query.Where(b => b.Tags.Any(t => t.Id == tagId));}

			if (voucherId > 0){ query = query.Where(b => b.Vouchers.Any(v => v.Id == voucherId));}

			if (imgId > 0){query = query.Where(b => b.Imgs.Any(i => i.Id == imgId));}

			if (!string.IsNullOrEmpty(key)){query = query.Where(b => b.Title.ToLower().Contains(key.ToLower()));}

			if (publisherId.HasValue){query = query.Where(b => b.PublisherId == publisherId.Value);}

			if (authorId.HasValue){query = query.Where(b => b.AuthorId == authorId.Value);}

			if (supplierId.HasValue){query = query.Where(b => b.SupplierId == supplierId.Value);}

			if (!includeDeleted){query = query.Where(b => !b.IsDeleted);}

			switch (sortBy.ToUpper())
			{
				case "TITLE": query = query.OrderBy(b => b.Title);break;
				case "PRICE": query = query.OrderBy(b => b.Price);break;
				case "PRICE_DEC": query = query.OrderByDescending(b => b.Price);break;
				case "PUBLISHDATE": query = query.OrderBy(b => b.PublishDate);break;
				case "ID":
				default: query = query.OrderBy(b => b.Id);break;
			}
		
			if (!includeDeleted){query = query.Where(b => !b.IsDeleted);}

			Total = query.Count();
			if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
			else
				return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
		}

		public async Task<List<Book>> GetCart(List<int> bookIds)
		{
			return await _dataContext.Books
		           .Where(b => bookIds.Contains(b.Id))
		           .ToListAsync();
		}

		public async Task<Book> GetBookById(int id)
		{
			return await _dataContext.Books
		         .Include(a => a.Author)
		         .Include(p => p.Publisher)
		         .Include(s => s.Supplier)
		         .Include(t => t.Tags)
		         .Include(v => v.Vouchers)
		         .Include(i => i.Imgs)
		         .Include(o => o.OrderDetails)
		         	.ThenInclude(od => od.Order)
		         .AsSplitQuery()
		         .FirstOrDefaultAsync(b => b.Id == id);
		}

		public async Task<ResponseDTO> UpdateBook(int id, Book book)
		{
			var existingBook = await _dataContext.Books.FindAsync(id);
			if (existingBook == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingBook.Title = book.Title;
			existingBook.Description = book.Description;
			existingBook.NumberOfPages = book.NumberOfPages;
			existingBook.PublishDate = book.PublishDate;
			existingBook.Language = book.Language;
			existingBook.Count = book.Count;
			existingBook.Price = book.Price;
			existingBook.MaxOrder = book.MaxOrder;
			existingBook.Format = book.Format;
			existingBook.PageSize = book.PageSize;
			existingBook.Update = DateTime.Now; 
			existingBook.IsDeleted = book.IsDeleted;
			existingBook.PublisherId = book.PublisherId;
			existingBook.AuthorId = book.AuthorId;
			existingBook.SupplierId = book.SupplierId;

			if (book.Tags != null && book.Tags.Count > 0) { existingBook.Tags = book.Tags;}
			if (book.Vouchers != null && book.Vouchers.Count > 0) { existingBook.Vouchers = book.Vouchers;}
			if (book.Imgs != null && book.Imgs.Count > 0){existingBook.Imgs = book.Imgs;}

			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
		}

		public async Task<ResponseDTO> DeleteBook(int id)
		{

			var book = await _dataContext.Books.FindAsync(id);
			if (book == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
			try
			{
				_dataContext.Books.Remove(book);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
			}
		}

		public async Task<ResponseDTO> CreateBook(Book book)
		{
			try
			{
				await _dataContext.Books.AddAsync(book);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}


		public async Task<List<Book>> GetTopOrderedBooks(int topCount = 10)
		{
	       	var topOrderedBooks = await _dataContext.Books
	           .Include(b => b.OrderDetails)
	       	   .ThenInclude(od => od.Order)
	           .Where(b => b.OrderDetails.Any(od => od.Order.Status == 4 && !od.Order.IsDeleted) && !b.IsDeleted)
	           .Select(b => new
	           {
	       	    Book = b,
	       	    TotalQuantity = b.OrderDetails
	       		.Where(od => od.Order.Status == 4 && !od.Order.IsDeleted)
	       		.Sum(od => od.Quantity)
	           })
	          .OrderByDescending(b => b.TotalQuantity)
	          .Take(topCount)
	          .Select(b => b.Book)
	          .ToListAsync();
	       
			return topOrderedBooks;
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}

		public async Task<int> GetBookCount()
		{
			return await _dataContext.Books.CountAsync(t => !t.IsDeleted);
		}
	}
}
