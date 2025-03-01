using AutoMapper;
using BookBee.DTO.Book;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.AuthorRepository;
using BookBee.Persistences.Repositories.CategoryRepository;
using BookBee.Persistences.Repositories.PublisherRepository;
using BookBee.Persistences.Repositories.SupplierRepository;
using BookBee.Persistences.Repositories.VoucherRepository;
using BookStack.Persistence.Repositories.BookRepository;

namespace BookStack.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IPublisherRepository _publisherRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IVoucherRepository _voucherRepository;

		private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper, ITagRepository tagRepository,
            IAuthorRepository authorRepository, IPublisherRepository publisherRepository,ISupplierRepository supplierRepository, IVoucherRepository voucherRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _tagRepository = tagRepository;
            _authorRepository = authorRepository;
            _publisherRepository = publisherRepository;
            _supplierRepository = supplierRepository;
            _voucherRepository = voucherRepository;
        }

 
          
		public async Task<ResponseDTO> CreateBook(BookDTO bookDTO)
		{
			var author = await _authorRepository.GetAuthorById(bookDTO.AuthorId);
			if (author == null) return new ResponseDTO(){Code = 400,Message = "Author không tồn tại"};
			var publisher = await _publisherRepository.GetPublisherById(bookDTO.PublisherId);
			if (publisher == null) return new ResponseDTO(){Code = 400,Message = "Publisher không tồn tại"};
			var supplier = await _supplierRepository.GetSupplierById(bookDTO.SupplierId);
			if (supplier == null) return new ResponseDTO() { Code = 400, Message = "Supplier không tồn tại" };
			if (string.IsNullOrEmpty(bookDTO.ImageUrl))
				return new ResponseDTO { Code = 400, Message = "Vui lòng upload ảnh trước" };

			var book = _mapper.Map<Book>(bookDTO);

			foreach (var tagId in bookDTO.TagIds)
			{
				Tag tag =await _tagRepository.GetTagById(tagId);
				if (tag != null)book.Tags.Add(tag);
			}
			if (book.Tags.Count == 0) return new ResponseDTO(){Code = 400,Message = "Tag không được để trống"};


			if (bookDTO.VoucherIds != null && bookDTO.VoucherIds.Any())
			{
				foreach (var voucherId in bookDTO.VoucherIds)
				{
					var voucher = await _voucherRepository.GetVoucherById(voucherId);
					if (voucher != null) book.Vouchers.Add(voucher);
				}
			}

			var books = new Book
			{
				Title = bookDTO.Title,
				Description =bookDTO.Description,
				NumberOfPages = bookDTO.NumberOfPages,
				PublishDate = bookDTO.PublishDate,
				Language =bookDTO.Language,
				Count = bookDTO.Count,
				Price = bookDTO.Price,
				Image = bookDTO.ImageUrl,
				Format =bookDTO.Format ,
				PageSize =bookDTO.PageSize,
				IsDeleted = false,
				PublisherId = bookDTO.PublisherId,
				AuthorId = bookDTO.AuthorId,
				SupplierId = bookDTO.SupplierId,
				Status = bookDTO.Status,
			};


			await _bookRepository.CreateBook(book);
			if (await _bookRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };

		}

		public async Task<ResponseDTO> DeleteBook(int id)
		{
			var book = await _bookRepository.GetBookById(id);
			if (book == null)
				return new ResponseDTO { Code = 400, Message = "Book không tồn tại" };
			if (book.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Book đã bị xóa trước đó" };

			book.IsDeleted = true;
			await _bookRepository.UpdateBook(id, book);
			bool isSaved = await _bookRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetBookById(int id)
		{
			var book = await _bookRepository.GetBookById(id);
			return book == null || book.IsDeleted
			? new ResponseDTO { Code = 400, Message = "Book không tồn tại hoặc đã bị xóa" }
			: new ResponseDTO { Code = 200, Message = "Lấy Book thành công", Data = _mapper.Map<BookDTO>(book) };

		}

		public async Task<ResponseDTO> GetBookByIds(List<int> ids)
		{
			if (ids == null || !ids.Any())
			{
				return new ResponseDTO { Code = 400, Message = "Danh sách ID không hợp lệ" };
			}

			var books = await _bookRepository.GetBookByIds(ids);

			if (books == null || !books.Any())
			{
				return new ResponseDTO { Code = 404, Message = "Không tìm thấy sách nào" };
			}

			return new ResponseDTO()
			{
				Data = _mapper.Map<List<BookDTO>>(books)
			};
		}

		public async Task<ResponseDTO> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0, bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? supplier = null, int? status = null)
		{
			if (page <= 0 || pageSize <= 0){return new ResponseDTO{Code = 400,Message = "Số trang và kích thước trang phải lớn hơn 0."};}
			if (status != null && status != 0 && status != 1){return new ResponseDTO{Code = 400,Message = "Trạng thái không hợp lệ. Chỉ chấp nhận 0 (Dừng) hoặc 1 (Hoạt động)."};}
			var books =await _bookRepository.GetBooks(page, pageSize, key, sortBy, tagId, voucherId, includeDeleted, publisherId, authorId,status);
			return new ResponseDTO(){Data = _mapper.Map<List<BookDTO>>(books),Total = _voucherRepository.Total};
		}

		public async Task<ResponseDTO> GetCart(List<int> bookIds)
		{
			if (bookIds == null || !bookIds.Any())
			{
				return new ResponseDTO { Code = 400, Message = "Danh sách sách trong giỏ hàng không hợp lệ" };
			}

			var books = await _bookRepository.GetCart(bookIds);

			if (books == null || !books.Any())
			{
				return new ResponseDTO { Code = 404, Message = "Không tìm thấy sách trong giỏ hàng" };
			}

			return new ResponseDTO(){Data = _mapper.Map<List<BookDTO>>(books),Total = books.Count};
		}

		public async Task<ResponseDTO> GetTopOrderedBooks(int topCount = 10)
		{
			var books = await _bookRepository.GetTopOrderedBooks(topCount);
			var data = _mapper.Map<List<BookDTO>>(books);
			return new ResponseDTO()
			{
				Data = data,
				Total = data.Count
			};
		}

		public async  Task<ResponseDTO> RestoreBook(int id)
		{
			var book = await _bookRepository.GetBookById(id);
			if (book == null){return new ResponseDTO{Code = 400,Message = "Book không tồn tại"};}
			book.IsDeleted = false;
		    await	_bookRepository.UpdateBook(id,book);
			var isSaved = await _bookRepository.IsSaveChanges();
			if (isSaved){return new ResponseDTO{Code = 200,Message = "Khôi phục thành công"};}
			else{return new ResponseDTO{Code = 500,Message = "Khôi phục thất bại, vui lòng thử lại sau"};}
		}

		public async Task<ResponseDTO> UpdateBook(int id, BookDTO BookDTO)
		{
			var book = await _bookRepository.GetBookById(id);
			if (book == null) return new ResponseDTO { Code = 400, Message = "Book không tồn tại" };


			book.Title = BookDTO.Title;
			book.Description = BookDTO.Description;
			book.NumberOfPages = BookDTO.NumberOfPages;
			book.PublishDate = BookDTO.PublishDate;
			book.Language = BookDTO.Language;
			book.Count = BookDTO.Count;
			book.Price = BookDTO.Price;
			book.Image = BookDTO.ImageUrl;
			book.Format = BookDTO.Format;
			book.PageSize = BookDTO.PageSize;
			book.PublisherId = BookDTO.PublisherId;
			book.AuthorId = BookDTO.AuthorId;
			book.SupplierId = BookDTO.SupplierId;
			book.Status = BookDTO.Status;

			book.Tags = new List<Tag>();
			foreach (var tagId in BookDTO.TagIds)
			{
				Tag tag =await _tagRepository.GetTagById(tagId);
				if (tag != null)
					book.Tags.Add(tag);
			}

			book.Vouchers = new List<Voucher>();
			if (BookDTO.VoucherIds != null && BookDTO.VoucherIds.Any())
			{
				foreach (var voucherId in BookDTO.VoucherIds)
				{
					var voucher = await _voucherRepository.GetVoucherById(voucherId);
					if (voucher != null)
					{
						book.Vouchers.Add(voucher);
					}
				}
			}

			await _bookRepository.UpdateBook(id, book);
			bool isSaved = await _bookRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}



		public async Task<ResponseDTO> BookStatus(int id, int status)
		{
			var book = await _bookRepository.GetBookById(id);
			if (book == null){return new ResponseDTO{Code = 400,Message = "Sách không tồn tại"};}
			if (status != 0 && status != 1){return new ResponseDTO{Code = 400,Message = "Trạng thái không hợp lệ. Chỉ chấp nhận 0 (Dừng) hoặc 1 (Hoạt động)."};}
			if (book.Status == status){return new ResponseDTO{Code = 400,Message = $"Sách đã ở trạng thái {(status == 1 ? "Hoạt động" : "Dừng hoạt động")} rồi."};}
			book.Status = status;
		    await _bookRepository.UpdateBook(id,book);
			var isSaved = await _bookRepository.IsSaveChanges();
			if (isSaved)
			{
				return new ResponseDTO{Code = 200,Message = $"Cập nhật trạng thái thành công: {(status == 1 ? "Hoạt động" : "Dừng hoạt động")}."};
			}
			else{return new ResponseDTO{Code = 500,Message = "Lỗi hệ thống! Không thể cập nhật trạng thái."};}
		}
	}
}
