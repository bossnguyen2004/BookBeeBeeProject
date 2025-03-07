﻿using BookBee.DTO.Response;
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

		public async Task<List<Book>> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0,  bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? supplierId = null, int? status = null)
		{
			var query = _dataContext.Books.Include(a => a.Author).Include(p => p.Publisher)
				.Include(s => s.Supplier).Include(t => t.Tags).Include(v => v.Vouchers)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Order)
				.AsSplitQuery().AsQueryable();


            if (tagId > 0){query = query.Where(t => t.Tags.Any(t => t.Id == tagId));}

            if (voucherId > 0)
            {
                bool isVoucherValid = await IsVoucherValid(voucherId.Value, 1);
                if (isVoucherValid)
                {
                    query = query.Where(b => b.Vouchers.Any(v => v.Id == voucherId));
                }
                else
                {
                    return new List<Book>(); 
                }
            }


            if (!string.IsNullOrEmpty(key)){query = query.Where(b => b.Title.ToLower().Contains(key.ToLower()));}

			if (publisherId.HasValue){query = query.Where(b => b.PublisherId == publisherId.Value);}

			if (authorId.HasValue){query = query.Where(b => b.AuthorId == authorId.Value);}

			if (supplierId.HasValue){query = query.Where(b => b.SupplierId == supplierId.Value);}
            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);
            else
                query = query.Where(b => b.Status == 1);
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

			Total = query.Count(); Total = query.Count();

            if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
			else
				return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
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
        public async Task<Book> GetBookById(int id)
        {
            return await _dataContext.Books
                 .Include(a => a.Author)
                 .Include(p => p.Publisher)
                 .Include(s => s.Supplier)
                 .Include(t => t.Tags)
                 .Include(v => v.Vouchers)
                 .Include(o => o.OrderDetails)
                     .ThenInclude(od => od.Order)
                 .AsSplitQuery()
                 .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<List<Book>> GetCart(List<int> bookIds)
		{
			return await _dataContext.Books
		           .Where(b => bookIds.Contains(b.Id))
		           .ToListAsync();
		}

		

		public async Task<ResponseDTO> UpdateBook(int id, Book book)
		{
			var existingBook = await _dataContext.Books.FindAsync(id);
			if (existingBook == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
            existingBook.CodeBook = book.CodeBook;
            existingBook.Title = book.Title;
			existingBook.Description = book.Description;
			existingBook.NumberOfPages = book.NumberOfPages;
			existingBook.PublishDate = book.PublishDate;
			existingBook.Language = book.Language;
			existingBook.Count = book.Count;
			existingBook.Price = book.Price;
            existingBook.GiaNhap = book.GiaNhap;
            existingBook.MaxOrder = book.MaxOrder;
			existingBook.Format = book.Format;
			existingBook.Image = book.Image;
			existingBook.PageSize = book.PageSize;
			existingBook.Update = DateTime.Now; 
			existingBook.IsDeleted = book.IsDeleted;
			existingBook.PublisherId = book.PublisherId;
			existingBook.AuthorId = book.AuthorId;
			existingBook.SupplierId = book.SupplierId;
            existingBook.GiaThucTe = book.GiaThucTe;
            if (book.StockQuantity.HasValue)
                existingBook.StockQuantity = book.StockQuantity;

            if (book.SoldQuantity.HasValue)
                existingBook.SoldQuantity = book.SoldQuantity;
            if (book.Tags != null && book.Tags.Count > 0) { existingBook.Tags = book.Tags;}
			if (book.Vouchers != null && book.Vouchers.Count > 0) { existingBook.Vouchers = book.Vouchers;}

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

		public async Task<List<Book>> GetBookByIds(List<int> ids)
		{
			return await _dataContext.Books
		       .Where(b => ids.Contains(b.Id)) 
		       .Include(a => a.Author)
		       .Include(p => p.Publisher)
		       .Include(s => s.Supplier)
		       .Include(t => t.Tags)
		       .Include(v => v.Vouchers)
		       .Include(o => o.OrderDetails)
		       	.ThenInclude(od => od.Order)
		       .AsSplitQuery()
		       .ToListAsync(); 
		}

        public async Task<List<Book>> GetInactiveBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0, bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? supplierId = null)
        {
            var query = _dataContext.Books.Include(a => a.Author).Include(p => p.Publisher)
                .Include(s => s.Supplier).Include(t => t.Tags).Include(v => v.Vouchers)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Order)
                .AsSplitQuery().AsQueryable();

            if (tagId > 0) query = query.Where(b => b.Tags.Any(t => t.Id == tagId));
            if (voucherId > 0) query = query.Where(b => b.Vouchers.Any(v => v.Id == voucherId));
            if (!string.IsNullOrEmpty(key)) query = query.Where(b => b.Title.ToLower().Contains(key.ToLower()));
            if (publisherId.HasValue) query = query.Where(b => b.PublisherId == publisherId.Value);
            if (authorId.HasValue) query = query.Where(b => b.AuthorId == authorId.Value);
            if (supplierId.HasValue) query = query.Where(b => b.SupplierId == supplierId.Value);

            query = query.Where(b => b.Status == 0);

            if (!includeDeleted) query = query.Where(b => !b.IsDeleted);

            switch (sortBy.ToUpper())
            {
                case "TITLE": query = query.OrderBy(b => b.Title); break;
                case "PRICE": query = query.OrderBy(b => b.Price); break;
                case "PRICE_DEC": query = query.OrderByDescending(b => b.Price); break;
                case "PUBLISHDATE": query = query.OrderBy(b => b.PublishDate); break;
                case "ID":
                default: query = query.OrderBy(b => b.Id); break;
            }

            Total = query.Count();
            return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
        }

        public async Task<List<Book>> GetLatestBooks(int count = 10)
        {
            return await _dataContext.Books
                         .OrderByDescending(b => b.PublishDate) 
                         .Take(count) 
                         .ToListAsync();
        }

        public async Task<ResponseDTO> RestoreBook(int id)
        {
            var book = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return new ResponseDTO
                { IsSuccess = false,Code = 404,Message = "Cuốn sách yêu cầu không tồn tại."
                };
            }

            if (!book.IsDeleted)
            {
                return new ResponseDTO
                {IsSuccess = false,Code = 400,Message = "Cuốn sách này đã hoạt động và không cần phục hồi."
                };
            }

            book.IsDeleted = false; 
            book.Update = DateTime.UtcNow; 

            await _dataContext.SaveChangesAsync();

            return new ResponseDTO
            {
                IsSuccess = true,Code = 200,Message = "Cuốn sách đã được khôi phục thành công.",
                Data = book
            };
        }



        public async Task<List<Book>> GetBooksByPriceRange(double fromPrice, double toPrice)
        {
            return await _dataContext.Books
                        .Where(b => b.Price >= fromPrice && b.Price <= toPrice && !b.IsDeleted)
                        .OrderBy(b => b.Price)
                       .ToListAsync();
        }

        public async  Task<bool> IsVoucherValid(int voucherId, int status)
        {
            var voucher = await _dataContext.Vouchers
         .FirstOrDefaultAsync(v => v.Id == voucherId);

            if (voucher == null || voucher.IsDeleted || voucher.Status != status)
            {
                return false;
            }

            if (voucher.EndDate < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
    }
}
